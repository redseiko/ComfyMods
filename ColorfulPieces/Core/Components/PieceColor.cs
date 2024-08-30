namespace ColorfulPieces;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;

using static ColorfulConstants;

public sealed class PieceColor : MonoBehaviour {
  public static readonly List<PieceColor> PieceColorCache = [];

  public Color TargetColor { get; set; } = Color.clear;
  public float TargetEmissionColorFactor { get; set; } = 0f;

  IPieceColorRenderer _pieceColorRenderer;

  int _cacheIndex;
  long _lastDataRevision;
  Vector3 _lastColorVec3;
  float _lastEmissionColorFactor;
  Color _lastColor;
  Color _lastEmissionColor;
  ZNetView _netView;

  void Awake() {
    _lastDataRevision = -1L;
    _lastColorVec3 = NoColorVector3;
    _lastEmissionColorFactor = NoEmissionColorFactor;
    _cacheIndex = -1;

    if (!TryGetComponent(out _netView) || !_netView || !_netView.IsValid()) {
      return;
    }

    PieceColorCache.Add(this);
    _cacheIndex = PieceColorCache.Count - 1;

    _pieceColorRenderer = GetPieceColorRenderer(_netView.m_zdo.m_prefab);
  }

  void OnDestroy() {
    if (_cacheIndex >= 0 && _cacheIndex < PieceColorCache.Count) {
      PieceColorCache[_cacheIndex] = PieceColorCache[PieceColorCache.Count - 1];
      PieceColorCache[_cacheIndex]._cacheIndex = _cacheIndex;
      PieceColorCache.RemoveAt(PieceColorCache.Count - 1);
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

      _pieceColorRenderer.SetColors(gameObject, TargetColor, TargetColor * TargetEmissionColorFactor);
    } else {
      TargetColor = Color.clear;
      TargetEmissionColorFactor = 0f;

      _pieceColorRenderer.ClearColors(gameObject);
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

    _pieceColorRenderer.SetColors(gameObject, color, emissionColor);
  }

  static IPieceColorRenderer GetPieceColorRenderer(int prefabHash) {
    if (prefabHash == GuardStoneHashCode) {
      // return GuardStonePieceColorRenderer.Instance;
    } else if (prefabHash == PortalWoodHashCode) {
      return PortalWoodPieceColorRenderer.Instance;
    }

    return DefaultPieceColorRenderer.Instance;
  }
}
