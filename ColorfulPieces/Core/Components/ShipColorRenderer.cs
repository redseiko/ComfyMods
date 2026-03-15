namespace ColorfulPieces;

using UnityEngine;

public interface IShipColorRenderer {
  void SetColors(GameObject targetObject, Color color, Color emissionColor);
  void ClearColors(GameObject targetObject);
}

public sealed class DefaultShipColorRenderer : IShipColorRenderer {
  public static DefaultShipColorRenderer Instance { get; } = new();

  public void SetColors(GameObject targetObject, Color color, Color emissionColor) {
    MaterialMan.s_instance.GetPropertyContainer(targetObject)
        .SetPropertyValue(ShaderProps._Color, color)
        .SetPropertyValue(ShaderProps._EmissionColor, emissionColor);
  }

  public void ClearColors(GameObject targetObject) {
    MaterialMan.s_instance.ResetValue(targetObject, ShaderProps._Color);
    MaterialMan.s_instance.ResetValue(targetObject, ShaderProps._EmissionColor);
  }
}
