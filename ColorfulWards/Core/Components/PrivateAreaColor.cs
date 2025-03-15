namespace ColorfulWards;

using ComfyLib;

using UnityEngine;

public sealed class PrivateAreaColor : MonoBehaviour {
  public static readonly int ColorShaderId = Shader.PropertyToID("_Color");
  public static readonly int EmissionColorShaderId = Shader.PropertyToID("_EmissionColor");

  PrivateArea _privateArea;
  ZNetView _netView;

  Color _currentColor;
  long _lastDataRevision;

  Light _pointLight;
  ParticleSystem _sparcsParticleSystem;
  ParticleSystem _flareParticleSystem;
  Renderer _glowMaterialRenderer;

  readonly MaterialPropertyBlock _propertyBlock = new();

  void Start() {
    _privateArea = GetComponent<PrivateArea>();

    if (!_privateArea || !_privateArea.m_nview || !_privateArea.m_nview.IsValid()) {
      return;
    }

    _netView = _privateArea.m_nview;

    _currentColor = PrivateAreaManager.NoColor;
    _lastDataRevision = -1L;

    _pointLight = transform.Find("WayEffect/Point light").GetComponent<Light>();
    _sparcsParticleSystem = transform.Find("WayEffect/sparcs").GetComponent<ParticleSystem>();
    _flareParticleSystem = transform.Find("WayEffect/flare").GetComponent<ParticleSystem>();
    _glowMaterialRenderer = transform.Find("new/default").GetComponent<Renderer>();
  }

  public void UpdateColors(bool forceUpdate = false) {
    if (!_netView || !_netView.IsValid()) {
      return;
    }

    long dataRevision = _netView.m_zdo.DataRevision;

    if (!forceUpdate && _lastDataRevision >= dataRevision) {
      return;
    }

    _lastDataRevision = dataRevision;

    if (!_netView.m_zdo.TryGetVector3(PrivateAreaManager.PrivateAreaColorHashCode, out Vector3 colorVector3)) {
      return;
    }

    Color privateAreaColor = PrivateAreaManager.Vector3ToColor(colorVector3);

    if (_netView.m_zdo.TryGetFloat(PrivateAreaManager.PrivateAreaColorAlphaHashCode, out float colorAlpha)) {
      privateAreaColor.a = colorAlpha;
    }

    if (privateAreaColor == _currentColor) {
      return;
    }

    SetPrivateAreaColors(privateAreaColor);
  }

  public void SetPrivateAreaColors(Color privateAreaColor) {
    _currentColor = privateAreaColor;
    _pointLight.color = privateAreaColor;

    _glowMaterialRenderer.GetPropertyBlock(_propertyBlock, 0);
    _propertyBlock.SetColor(EmissionColorShaderId, privateAreaColor);
    _glowMaterialRenderer.SetPropertyBlock(_propertyBlock, 0);

    // WayEffect/sparcs
    ParticleSystem.MainModule main = _sparcsParticleSystem.main;
    main.startColor = new(privateAreaColor);

    ParticleSystem.ColorOverLifetimeModule colorOverLifetime = _sparcsParticleSystem.colorOverLifetime;
    colorOverLifetime.color = new(privateAreaColor);

    _sparcsParticleSystem.Restart();

    // WayEffect/flare
    main = _flareParticleSystem.main;
    main.startColor = new(privateAreaColor.SetAlpha(0.1f));

    _flareParticleSystem.Restart();
  }
}
