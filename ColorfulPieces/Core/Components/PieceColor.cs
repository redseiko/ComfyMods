using System.Collections.Generic;

using UnityEngine;

using static ColorfulPieces.PluginConstants;

namespace ColorfulPieces {
  public class PieceColor : MonoBehaviour {
    public static readonly Dictionary<string, int> RendererCountCache = new();
    public static readonly List<PieceColor> PieceColorCache = new();

    public Color TargetColor { get; set; } = Color.clear;
    public float TargetEmissionColorFactor { get; set; } = 0f;

    readonly List<Renderer> _renderers = new(0);
    IPieceColorRenderer _pieceColorRenderer;

    int _cacheIndex;
    long _lastDataRevision;
    Vector3 _lastColorVec3;
    float _lastEmissionColorFactor;
    Color _lastColor;
    Color _lastEmissionColor;
    ZNetView _netView;

    void Awake() {
      _renderers.Clear();

      _lastDataRevision = -1L;
      _lastColorVec3 = NoColorVector3;
      _lastEmissionColorFactor = NoEmissionColorFactor;
      _cacheIndex = -1;

      _netView = GetComponent<ZNetView>();

      if (!_netView || !_netView.IsValid()) {
        return;
      }

      PieceColorCache.Add(this);
      _cacheIndex = PieceColorCache.Count - 1;

      CacheRenderers();
      _pieceColorRenderer = GetPieceColorRenderer(gameObject.name);
    }

    static IPieceColorRenderer GetPieceColorRenderer(string prefabName) {
      return prefabName switch {
        "guard_stone(Clone)" => new GuardStonePieceColorRenderer(),
        "portal_wood(Clone)" => new PortalWoodPieceColorRenderer(),
        _ => new DefaultPieceColorRenderer(),
      };
    }

    void OnDestroy() {
      if (_cacheIndex >= 0 && _cacheIndex < PieceColorCache.Count) {
        PieceColorCache[_cacheIndex] = PieceColorCache[PieceColorCache.Count - 1];
        PieceColorCache[_cacheIndex]._cacheIndex = _cacheIndex;
        PieceColorCache.RemoveAt(PieceColorCache.Count - 1);
      }

      _renderers.Clear();
    }

    void CacheRenderers() {
      if (RendererCountCache.TryGetValue(gameObject.name, out int count)) {
        _renderers.Capacity = count;
      }

      _renderers.AddRange(gameObject.GetComponentsInChildren<MeshRenderer>(true));
      _renderers.AddRange(gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true));

      if (_renderers.Count != count) {
        RendererCountCache[gameObject.name] = _renderers.Count;
        _renderers.Capacity = _renderers.Count;
      }
    }

    public void UpdateColors(bool forceUpdate = false) {
      if (!_netView || !_netView.IsValid()) {
        return;
      }

      if (!forceUpdate && _lastDataRevision >= _netView.m_zdo.DataRevision) {
        return;
      }

      bool isColored = true;
      _lastDataRevision = _netView.m_zdo.DataRevision;

      if (!_netView.m_zdo.TryGetVector3(PieceColorHashCode, out Vector3 colorVec3)
          || colorVec3 == NoColorVector3) {
        colorVec3 = NoColorVector3;
        isColored = false;
      }

      if (!_netView.m_zdo.TryGetFloat(PieceEmissionColorFactorHashCode, out float factor)
          || factor == NoEmissionColorFactor) {
        factor = NoEmissionColorFactor;
        isColored = false;
      }

      if (!forceUpdate && colorVec3 == _lastColorVec3 && factor == _lastEmissionColorFactor) {
        return;
      }

      _lastColorVec3 = colorVec3;
      _lastEmissionColorFactor = factor;

      if (isColored) {
        TargetColor = Vector3ToColor(colorVec3);
        TargetEmissionColorFactor = factor;

        _pieceColorRenderer.SetColors(_renderers, TargetColor, TargetColor * TargetEmissionColorFactor);
      } else {
        TargetColor = Color.clear;
        TargetEmissionColorFactor = 0f;

        _pieceColorRenderer.ClearColors(_renderers);
      }

      _lastColor = TargetColor;
      _lastEmissionColor = TargetColor * TargetEmissionColorFactor;
    }

    public void OverrideColors(Color color, Color emissionColor) {
      if (color == _lastColor && emissionColor == _lastEmissionColor) {
        return;
      }

      _lastColor = color;
      _lastEmissionColor = emissionColor;

      _pieceColorRenderer.SetColors(_renderers, color, emissionColor);
    }
  }
}
