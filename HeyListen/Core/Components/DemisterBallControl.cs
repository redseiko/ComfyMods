namespace HeyListen;

using System.Collections;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public sealed class DemisterBallControl : MonoBehaviour {
  public static readonly Color NoColor = new(-1f, -1f, -1f, -1f);

  public ZNetView NetView { get; private set; }

  long _lastDataRevision;
  float _lastBodyScale;
  Color _lastBodyColor;
  float _lastBodyBrightness;
  Color _lastPointLightColor;

  RendererSetting _demisterBallRenderer;
  LightSetting _effectsPointLight;
  ParticleSystemSetting _flameEffectsFlames;
  ParticleSystemSetting _flameEffectsFlames2;
  ParticleSystemSetting _flameEffectsFlare;
  RendererSetting _flameEffectsEmbers;
  RendererSetting _flameEffectsDistortion;
  RendererSetting _flameEffectsEnergy;
  ParticleSystemSetting _flameEffectsEnergy2;
  RendererSetting _flameEffectsSparcs;

  void Awake() {
    _lastDataRevision = -1L;
    _lastBodyScale = -1f;
    _lastBodyColor = NoColor;
    _lastBodyBrightness = -1f;
    _lastPointLightColor = NoColor;

    _demisterBallRenderer = new(transform.Find("demister_ball").GetComponent<MeshRenderer>());
    _effectsPointLight = new(transform.Find("effects/Point light").GetComponent<Light>());

    _flameEffectsFlames = new(transform.Find("effects/flame/flames").GetComponent<ParticleSystem>());
    _flameEffectsFlames2 = new(transform.Find("effects/flame/flames_local").GetComponent<ParticleSystem>());
    _flameEffectsFlare = new(transform.Find("effects/flame/flare").GetComponent<ParticleSystem>());
    _flameEffectsEmbers = new(transform.Find("effects/flame/embers").GetComponent<ParticleSystemRenderer>());
    _flameEffectsDistortion = new(transform.Find("effects/flame/distortiion").GetComponent<ParticleSystemRenderer>());
    _flameEffectsEnergy = new(transform.Find("effects/flame/energy").GetComponent<ParticleSystemRenderer>());
    _flameEffectsEnergy2 = new(transform.Find("effects/flame/energy (1)").GetComponent<ParticleSystem>());
    _flameEffectsSparcs = new(transform.Find("effects/flame/sparcs_front").GetComponent<ParticleSystemRenderer>());      

    NetView = GetComponent<ZNetView>();

    if (!NetView || !NetView.IsValid()) {
      return;
    }

    StartCoroutine(UpdateDemisterBallCoroutine());
  }

  void OnDestroy() {
    transform.localScale = Vector3.one;

    _demisterBallRenderer.Reset();
    _effectsPointLight.Reset();

    _flameEffectsFlames.Reset();
    _flameEffectsFlames2.Reset();
    _flameEffectsFlare.Reset();
    _flameEffectsEmbers.Reset();
    _flameEffectsDistortion.Reset();
    _flameEffectsEnergy.Reset();
    _flameEffectsEnergy2.Reset();
    _flameEffectsSparcs.Reset();
  }

  IEnumerator UpdateDemisterBallCoroutine() {
    WaitForSeconds waitInterval = new(seconds: 2f);

    while (NetView && NetView.IsValid()) {
      UpdateDemisterBall(forceUpdate: false);
      yield return waitInterval;
    }
  }

  public void UpdateDemisterBall(bool forceUpdate = false) {
    if (!NetView || !NetView.IsValid() || !IsModEnabled.Value) {
      return;
    }

    if (!forceUpdate && _lastDataRevision >= NetView.m_zdo.DataRevision) {
      return;
    }

    _lastDataRevision = NetView.m_zdo.DataRevision;

    UpdateBodyScale(forceUpdate);
    UpdateBodyColor(forceUpdate);
    UpdatePointLightColor(forceUpdate);
    UpdateFlameEffects();
  }

  void UpdateBodyScale(bool forceUpdate = false) {
    float scale = NetView.m_zdo.GetFloat(WispManager.DemisterBallBodyScaleHashCode, -1f);

    if (!forceUpdate && scale == _lastBodyScale) {
      return;
    }

    _lastBodyScale = scale;
    Vector3 localScale = Vector3.one * scale;

    transform.localScale = localScale;
    _flameEffectsFlames.SetScale(localScale);
    _flameEffectsFlames2.SetScale(localScale);
    //_flameEffectsFlare.SetScale(localScale);
    //_flameEffectsEmbers.SetScale(localScale);
    //_flameEffectsDistortion.SetScale(localScale);
    //_flameEffectsEnergy.SetScale(localScale);
    //_flameEffectsEnergy2.SetScale(localScale);
    //_flameEffectsSparcs.SetScale(localScale);
  }

  void UpdateBodyColor(bool forceUpdate = false) {
    Color color = NetView.m_zdo.GetColor(WispManager.DemisterBallBodyColorHashCode, NoColor);

    float brightness =
        Mathf.Clamp(NetView.m_zdo.GetFloat(WispManager.DemisterBallBodyBrightnessHashCode, -1f), -1f, 2f);

    if (!forceUpdate && color == _lastBodyColor && brightness == _lastBodyBrightness) {
      return;
    }

    if (brightness >= 0f && color != NoColor) {
      _demisterBallRenderer.SetColor(color);
      _demisterBallRenderer.SetEmissionColor(color * new Color(brightness, brightness, brightness, 1f));
    } else {
      _demisterBallRenderer.SetColor(_demisterBallRenderer.OriginalColor);
      _demisterBallRenderer.SetEmissionColor(_demisterBallRenderer.OriginalEmissionColor);
    }

    _lastBodyColor = color;
    _lastBodyBrightness = brightness;
  }

  void UpdatePointLightColor(bool forceUpdate) {
    Color color = NetView.m_zdo.GetColor(WispManager.DemisterBallPointLightColorHashCode, NoColor);

    if (!forceUpdate && color == _lastPointLightColor) {
      return;
    }

    _effectsPointLight.SetColor(color == NoColor ? _effectsPointLight.OriginalColor : color);
    _lastPointLightColor = color;
  }

  public void UpdateFlameEffects() {
    FlameEffects effectsEnabled =
         (FlameEffects) NetView.m_zdo.GetInt(WispManager.FlameEffectsEnabledHashCode, (int) DefaultFlameEffects);

    _flameEffectsFlames.SetActive(effectsEnabled.HasFlag(FlameEffects.Flames));
    _flameEffectsFlames2.SetActive(effectsEnabled.HasFlag(FlameEffects.FlamesL));

    // ParticleSystem.main.startColor: keep alpha to 0.1 or less
    _flameEffectsFlare.SetActive(effectsEnabled.HasFlag(FlameEffects.Flare));

    // ParticleSystemRenderer.material._EmissionColor: drives this color
    _flameEffectsDistortion.SetActive(effectsEnabled.HasFlag(FlameEffects.Distortion));

    // ParticleSystemRenderer.material._EmissionColor: drives this color
    _flameEffectsEmbers.SetActive(effectsEnabled.HasFlag(FlameEffects.Embers));

    // ParticleSystemRenderer.material._EmissionColor: drives this color
    _flameEffectsDistortion.SetActive(effectsEnabled.HasFlag(FlameEffects.Distortion));

    // ParticleSystemRenderer.material._EmissionColor: drives this color
    _flameEffectsEnergy.SetActive(effectsEnabled.HasFlag(FlameEffects.Energy));

    // ParticleSystem.main.startColor: keep alpha to 0.1 or less
    _flameEffectsEnergy2.SetActive(effectsEnabled.HasFlag(FlameEffects.EnergyII));

    // ParticleSystemRenderer.material._EmissionColor: drives this color
    _flameEffectsSparcs.SetActive(effectsEnabled.HasFlag(FlameEffects.Sparcs));

    if (NetView.m_zdo.TryGetColor(WispManager.FlameEffectsColorHashCode, out Color effectsColor)) {
      _flameEffectsFlames.SetColorOverLifetimeColor(effectsColor);
      _flameEffectsFlames2.SetColorOverLifetimeColor(effectsColor);
      _flameEffectsFlare.SetStartColor(effectsColor.SetAlpha(0.1f)); // <--
      _flameEffectsEnergy2.SetStartColor(effectsColor.SetAlpha(0.1f)); // <--
    } else {
      _flameEffectsFlames.SetColorOverLifetimeColor(_flameEffectsFlames.OriginalColorOveLifetimeColor);
      _flameEffectsFlames2.SetColorOverLifetimeColor(_flameEffectsFlames2.OriginalColorOveLifetimeColor);
      _flameEffectsFlare.SetStartColor(_flameEffectsFlare.OriginalStartColor);
      _flameEffectsEnergy2.SetStartColor(_flameEffectsEnergy2.OriginalStartColor);
    }

    UpdateFlameEffectsRenderer(
        _flameEffectsEmbers,
        WispManager.FlameEffectsEmbersColorHashCode,
        WispManager.FlameEffectsEmbersBrightnessHashCode);

    UpdateFlameEffectsRenderer(
        _flameEffectsSparcs,
        WispManager.FlameEffectsSparcsColorHashCode,
        WispManager.FlameEffectsSparcsBrightnessHashCode);
  }

  void UpdateFlameEffectsRenderer(RendererSetting rendererSetting, int colorHashCode, int brightnessHashCode) {
    if (NetView.m_zdo.TryGetColor(colorHashCode, out Color color)
        && NetView.m_zdo.TryGetFloat(brightnessHashCode, out float brightness)) {
      rendererSetting
          .SetColor(color)
          .SetEmissionColor(color * new Color(brightness, brightness, brightness, 1f));
    } else {
      rendererSetting
          .SetColor(rendererSetting.OriginalColor)
          .SetEmissionColor(rendererSetting.OriginalEmissionColor);
    }
  }
}
