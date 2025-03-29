namespace Dramamist;

using UnityEngine;

public sealed class ParticleSystemProfile {
  public ParticleSystem.MinMaxGradient StartColor { get; }
  public ParticleSystem.MinMaxCurve StartRotation { get; }

  public ParticleSystem.MinMaxCurve VelocityOverLifetimeX { get; }
  public ParticleSystem.MinMaxCurve VelocityOverLifetimeY { get; }
  public ParticleSystem.MinMaxCurve VelocityOverLifetimeZ { get; }

  public ParticleSystem.MinMaxCurve RotationOverLifetimeX { get; }
  public ParticleSystem.MinMaxCurve RotationOverLifetimeY { get; }
  public ParticleSystem.MinMaxCurve RotationOverLifetimeZ { get; }

  public ParticleSystemProfile(ParticleSystem particleSystem) {
    ParticleSystem.MainModule main = particleSystem.main;
    StartRotation = main.startRotation;
    StartColor = main.startColor;

    ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = particleSystem.velocityOverLifetime;
    VelocityOverLifetimeX = velocityOverLifetime.x;
    VelocityOverLifetimeY = velocityOverLifetime.y;
    VelocityOverLifetimeZ = velocityOverLifetime.z;

    ParticleSystem.RotationOverLifetimeModule rotationOverLifetime = particleSystem.rotationOverLifetime;
    RotationOverLifetimeX = rotationOverLifetime.x;
    RotationOverLifetimeY = rotationOverLifetime.y;
    RotationOverLifetimeZ = rotationOverLifetime.z;
  }
}
