namespace ColorfulPortals;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;

public sealed class TeleportWorldColor : MonoBehaviour {
  public static readonly List<TeleportWorldColor> TeleportWorldColorCache = [];

  TeleportWorld _teleportWorld;
  ZNetView _netView;
  Color _currentColor;
  long _lastDataRevision;

  Light _pointLight;
  ParticleSystem _suckParticles;
  ParticleSystem _particleSystem;
  ParticleSystem _blueFlames;

  void Awake() {
    _teleportWorld = GetComponent<TeleportWorld>();

    if (!_teleportWorld || !_teleportWorld.m_nview || !_teleportWorld.m_nview.IsValid()) {
      return;
    }

    _netView = _teleportWorld.m_nview;

    _currentColor = TeleportWorldManager.NoColor;
    _lastDataRevision = -1L;

    _pointLight = transform.Find("_target_found_red/Point light").GetComponent<Light>();
    _suckParticles =
        transform.Find("_target_found_red/Particle System/suck particles").GetComponent<ParticleSystem>();
    _particleSystem = transform.Find("_target_found_red/Particle System").GetComponent<ParticleSystem>();
    _blueFlames = transform.Find("_target_found_red/Particle System/blue flames").GetComponent<ParticleSystem>();

    TeleportWorldColorCache.Add(this);
  }

  void OnDestroy() {
    TeleportWorldColorCache.Remove(this);
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

    if (!_netView.m_zdo.TryGetVector3(TeleportWorldManager.TeleportWorldColorHashCode, out Vector3 colorVector3)) {
      return;
    }

    Color portalColor = TeleportWorldManager.Vector3ToColor(colorVector3);

    if (_netView.m_zdo.TryGetFloat(TeleportWorldManager.TeleportWorldColorAlphaHashCode, out float colorAlpha)) {
      portalColor.a = colorAlpha;
    }

    if (portalColor == _currentColor) {
      return;
    }

    SetPortalColors(portalColor);
  }

  public void SetPortalColors(Color portalColor) {
    _currentColor = portalColor;
    _pointLight.color = portalColor;

    SetParticleSystemColor(_suckParticles, portalColor);
    SetParticleSystemColor(_particleSystem, portalColor);
    SetParticleSystemColor(_blueFlames, portalColor);
  }

  void SetParticleSystemColor(ParticleSystem ps, Color portalColor) {
    ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ps.colorOverLifetime;

    if (colorOverLifetime.enabled) {
      colorOverLifetime.color = new(portalColor);
    }

    ParticleSystem.MainModule main = ps.main;
    main.startColor = new(portalColor);

    ParticleSystem.CustomDataModule customData = ps.customData;

    if (customData.enabled) {
      customData.SetColor(ParticleSystemCustomData.Custom1, new(portalColor));
    }

    ps.Clear();
    ps.Simulate(0f);
    ps.Play();
  }
}
