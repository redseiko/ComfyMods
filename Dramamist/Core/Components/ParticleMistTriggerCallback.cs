namespace Dramamist;

using System.Collections.Generic;

using UnityEngine;

using static PluginConfig;

public sealed class ParticleMistTriggerCallback : MonoBehaviour {
  ParticleSystem _particleSystem;
  readonly List<ParticleSystem.Particle> _insideParticles = [];

  void OnEnable() {
    _particleSystem = GetComponent<ParticleSystem>();

    ParticleSystem.MainModule main = _particleSystem.main;
    _insideParticles.Capacity = main.maxParticles;
  }

  void OnParticleTrigger() {
    if (IsModEnabled.Value) {
      UpdateParticles(ParticleSystemTriggerEventType.Inside);
    }
  }

  void UpdateParticles(ParticleSystemTriggerEventType eventType) {
    float multiplier = DemisterTriggerFadeOutMultiplier.Value;
    int count = _particleSystem.GetTriggerParticles(eventType, _insideParticles);

    for (int i = 0; i < count; i++) {
      ParticleSystem.Particle particle = _insideParticles[i];
      Color color = particle.startColor;
      color.a *= multiplier;
      particle.startColor = color;

      _insideParticles[i] = particle;
    }

    _particleSystem.SetTriggerParticles(eventType, _insideParticles);
  }
}
