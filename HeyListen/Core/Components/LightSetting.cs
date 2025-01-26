namespace ComfyLib;

using UnityEngine;

public sealed class LightSetting {
  public Vector3 OriginalScale { get; }
  public Vector3 CurrentScale { get; private set; }

  public Color OriginalColor { get; }
  public Color CurrentColor { get; private set; }

  readonly Light _light;

  public LightSetting(Light light) {
    _light = light;

    OriginalScale = light.transform.localScale;
    CurrentScale = OriginalScale;

    OriginalColor = light.color;
    CurrentColor = OriginalColor;
  }

  public LightSetting Reset() {
    SetScale(OriginalScale);
    SetColor(OriginalColor);

    return this;
  }

  public LightSetting SetScale(Vector3 scale) {
    if (scale != CurrentScale) {
      CurrentScale = scale;
      _light.transform.localScale = scale;
    }

    return this;
  }

  public LightSetting SetColor(Color color) {
    if (color != CurrentColor) {
      CurrentColor = color;
      _light.color = color;
    }

    return this;
  }
}
