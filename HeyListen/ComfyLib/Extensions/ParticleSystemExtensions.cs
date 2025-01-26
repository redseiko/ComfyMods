namespace ComfyLib;

using System.Linq;

using UnityEngine;

public static class ParticleSystemExtensions {
  public static ParticleSystem.MainModule Main(this ParticleSystem particleSystem) {
    return particleSystem.main;
  }

  public static ParticleSystem.MainModule SetStartColor(
      this ParticleSystem.MainModule main, ParticleSystem.MinMaxGradient color) {
    main.startColor = color;
    return main;
  }

  public static ParticleSystem.ColorOverLifetimeModule ColorOverLifetime(this ParticleSystem particleSystem) {
    return particleSystem.colorOverLifetime;
  }

  public static ParticleSystem.ColorOverLifetimeModule SetColor(
      this ParticleSystem.ColorOverLifetimeModule colorOverLifetime, ParticleSystem.MinMaxGradient color) {
    colorOverLifetime.color = color;
    return colorOverLifetime;
  }

  public static string ToDebugString(this ParticleSystem.MinMaxGradient minMaxGradient) {
    return minMaxGradient.mode switch {
       ParticleSystemGradientMode.Color => $"Color: {minMaxGradient.color:F3}",
       ParticleSystemGradientMode.TwoColors =>
          $"TwoColor: ColorMin ({minMaxGradient.colorMin:F3}), ColorMax ({minMaxGradient.colorMax:F3})",
       ParticleSystemGradientMode.Gradient => $"Gradient: {minMaxGradient.gradient.ToDebugString()}",
       ParticleSystemGradientMode.TwoGradients =>
          $"TwoGradients: "
              + $"GradientMin ({minMaxGradient.gradientMin.ToDebugString()}), "
              + $"GradientMax ({minMaxGradient.gradientMax.ToDebugString()})",
       ParticleSystemGradientMode.RandomColor => $"RandomColor: {minMaxGradient.gradient.ToDebugString()}",
       _ => $"{minMaxGradient}"
    };
  }

  public static string ToDebugString(this Gradient gradient) {
    return string.Format(
        "ColorKeys = [{0}], AlphaKeys = [{1}]",
        string.Join(", ", gradient.colorKeys.Select(key => $"({key.time:F3} {key.color:F3})")),
        string.Join(", ", gradient.alphaKeys.Select(key => $"({key.time:F3} {key.alpha:F3})")));
  }
}
