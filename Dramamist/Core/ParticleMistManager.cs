namespace Dramamist;

using UnityEngine;

using static PluginConfig;

public static class ParticleMistManager {
  static readonly ParticleSystem.MinMaxCurve _zeroCurve = new(0f);

  static ParticleSystemProfile _particleMistProfile;
  static ParticleSystem.MinMaxGradient _flatStartColor;

  public static void UpdateDemisterSettings() {
    if (Demister.m_instances.Count <= 0) {
      return;
    }

    foreach (Demister demister in Demister.m_instances) {
      UpdateDemisterSettings(demister);
    }
  }

  public static void UpdateDemisterSettings(Demister demister) {
    demister.m_forceField.gravity = IsModEnabled.Value ? DemisterForceFieldGravity.Value : -0.08f;

    if (demister.TryGetComponent(out FadeOutParticleMist fadeOut)) {
      fadeOut.enabled = IsModEnabled.Value && DemisterTriggerFadeOutParticleMist.Value;
    }
  }

  public static void UpdateParticleMistSettings() {
    if (!ParticleMist.m_instance) {
      return;
    }

    ParticleMist particleMist = ParticleMist.m_instance;
    ParticleSystem.MainModule main = particleMist.m_ps.main;
    ParticleSystem.TriggerModule trigger = particleMist.m_ps.trigger;
    ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = particleMist.m_ps.velocityOverLifetime;
    ParticleSystem.RotationOverLifetimeModule rotationOverLifetime = particleMist.m_ps.rotationOverLifetime;

    if (_particleMistProfile == null) {
      _particleMistProfile ??= new(particleMist.m_ps);
      _flatStartColor = new ParticleSystem.MinMaxGradient(main.startColor.colorMax);
    }

    if (IsModEnabled.Value && ParticleMistReduceMotion.Value) {
      main.startRotation = _zeroCurve;

      velocityOverLifetime.x = _zeroCurve;
      velocityOverLifetime.y = _zeroCurve;
      velocityOverLifetime.z = _zeroCurve;

      rotationOverLifetime.x = _zeroCurve;
      rotationOverLifetime.y = _zeroCurve;
      rotationOverLifetime.z = _zeroCurve;
    } else {
      main.startRotation = _particleMistProfile.StartRotation;

      velocityOverLifetime.x = _particleMistProfile.VelocityOverLifetimeX;
      velocityOverLifetime.y = _particleMistProfile.VelocityOverLifetimeY;
      velocityOverLifetime.z = _particleMistProfile.VelocityOverLifetimeZ;

      rotationOverLifetime.x = _particleMistProfile.RotationOverLifetimeX;
      rotationOverLifetime.y = _particleMistProfile.RotationOverLifetimeY;
      rotationOverLifetime.z = _particleMistProfile.RotationOverLifetimeZ;
    }

    if (IsModEnabled.Value && ParticleMistUseFlatMistStartColor.Value) {
      main.startColor = _flatStartColor;
    } else {
      main.startColor = _particleMistProfile.StartColor;
    }

    trigger.enabled = IsModEnabled.Value && DemisterTriggerFadeOutParticleMist.Value;
    trigger.inside = ParticleSystemOverlapAction.Callback;
    trigger.colliderQueryMode = ParticleSystemColliderQueryMode.Disabled;
  }
}
