namespace ColorfulLights;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public sealed class FireplaceColor : MonoBehaviour {
  public static long TotalCount { get; private set; } = 0L;
  public static long CurrentCount { get; private set; } = 0L;

  public Color TargetColor { get; private set; } = Color.clear;

  private Vector3 _targetColorVec3;
  private float _targetColorAlpha;

  private ZNetView _netView;

  private readonly List<Light> _lights = [];
  private readonly List<ParticleSystem> _systems = [];
  private readonly List<ParticleSystemRenderer> _renderers = [];

  private void Awake() {
    TotalCount++;
    CurrentCount++;

    TargetColor = Color.clear;

    _targetColorVec3 = Vector3.positiveInfinity;
    _targetColorAlpha = float.NaN;

    _lights.Clear();
    _systems.Clear();
    _renderers.Clear();

    Fireplace fireplace = GetComponent<Fireplace>();

    if (!fireplace || !fireplace.m_nview || !fireplace.m_nview.IsValid()) {
      return;
    }

    _netView = fireplace.m_nview;

    CacheComponents(fireplace.m_enabledObject);
    CacheComponents(fireplace.m_enabledObjectHigh);
    CacheComponents(fireplace.m_enabledObjectLow);

    InvokeRepeating(nameof(UpdateColors), 0f, 3f);
  }

  private void OnDestroy() {
    CurrentCount--;
  }

  private void CacheComponents(GameObject target) {
    if (target) {
      _lights.AddRange(target.GetComponentsInChildren<Light>(includeInactive: true));
      _systems.AddRange(target.GetComponentsInChildren<ParticleSystem>(includeInactive: true));
      _renderers.AddRange(target.GetComponentsInChildren<ParticleSystemRenderer>(includeInactive: true));
    }
  }

  public void UpdateColors() {
    if (!_netView || !_netView.IsValid() || !IsModEnabled.Value) {
      CancelInvoke(nameof(UpdateColors));
      return;
    }

    if (!_netView.m_zdo.TryGetVector3(FireplaceManager.FirePlaceColorHashCode, out Vector3 colorVec3)) {
      return;
    }

    float colorAlpha = _netView.m_zdo.GetFloat(FireplaceManager.FireplaceColorAlphaHashCode, 1f);

    if (colorVec3 == _targetColorVec3 && colorAlpha == _targetColorAlpha) {
      return;
    }

    SetFireplaceColors(colorVec3, colorAlpha);
  }

  public void SetFireplaceColors(Vector3 colorVec3, float colorAlpha) {
    TargetColor = Utils.Vec3ToColor(colorVec3).SetAlpha(colorAlpha);

    _targetColorVec3 = colorVec3;
    _targetColorAlpha = colorAlpha;

    SetParticleColors(TargetColor);
  }

  void SetParticleColors(Color color) {
    ParticleSystem.MinMaxGradient gradient = new(color);

    foreach (ParticleSystem system in _systems) {
      ParticleSystem.ColorOverLifetimeModule colorOverLiftime = system.colorOverLifetime;

      if (colorOverLiftime.enabled) {
        colorOverLiftime.color = gradient;
      }

      ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = system.sizeOverLifetime;

      if (sizeOverLifetime.enabled) {
        ParticleSystem.MainModule main = system.main;
        main.startColor = color;
      }
    }

    foreach (ParticleSystemRenderer renderer in _renderers) {
      renderer.material.color = color;
    }

    foreach (Light light in _lights) {
      light.color = color;
    }
  }
}
