namespace ComfyLib;

using UnityEngine;

public sealed class ParticleSystemSetting {
  public Vector3 OriginalScale { get; }
  public Vector3 CurrentScale { get; private set; }

  public ParticleSystem.MinMaxGradient OriginalStartColor { get; }
  public ParticleSystem.MinMaxGradient CurrentStartColor { get; private set; }

  public ParticleSystem.MinMaxGradient OriginalColorOveLifetimeColor { get; }
  public ParticleSystem.MinMaxGradient CurrentColorOverLifetimeColor { get; private set; }

  readonly ParticleSystem _particleSystem;

  public ParticleSystemSetting(ParticleSystem particleSystem) {
    _particleSystem = particleSystem;

    OriginalScale = particleSystem.transform.localScale;
    CurrentScale = OriginalScale;

    ParticleSystem.MainModule main = particleSystem.main;
    OriginalStartColor = main.startColor;
    CurrentStartColor = OriginalStartColor;

    ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
    OriginalColorOveLifetimeColor = colorOverLifetime.color;
    CurrentColorOverLifetimeColor = OriginalColorOveLifetimeColor;
  }

  public ParticleSystemSetting Reset() {
    SetScale(OriginalScale);
    SetStartColor(OriginalStartColor);
    SetColorOverLifetimeColor(OriginalColorOveLifetimeColor);

    return this;
  }

  public ParticleSystemSetting SetActive(bool active) {
    _particleSystem.gameObject.SetActive(active);
    return this;
  }

  public ParticleSystemSetting SetScale(Vector3 scale) {
    if (scale != CurrentScale) {
      CurrentScale = scale;
      _particleSystem.transform.localScale = scale;
    }

    return this;
  }

  public ParticleSystemSetting SetStartColor(Color color) {
    return SetStartColor(new ParticleSystem.MinMaxGradient(color));
  }

  public ParticleSystemSetting SetStartColor(ParticleSystem.MinMaxGradient color) {
    CurrentStartColor = color;
    _particleSystem.Main().SetStartColor(CurrentStartColor);

    return this;
  }

  public ParticleSystemSetting SetColorOverLifetimeColor(Color color) {
    return SetColorOverLifetimeColor(new ParticleSystem.MinMaxGradient(color));
  }

  public ParticleSystemSetting SetColorOverLifetimeColor(ParticleSystem.MinMaxGradient color) {
    CurrentColorOverLifetimeColor = color;
    _particleSystem.ColorOverLifetime().SetColor(CurrentColorOverLifetimeColor);

    return this;
  }
}
