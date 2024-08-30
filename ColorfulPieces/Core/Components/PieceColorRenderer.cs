namespace ColorfulPieces;

using UnityEngine;

public interface IPieceColorRenderer {
  void SetColors(GameObject targetObject, Color color, Color emissionColor);
  void ClearColors(GameObject targetObject);
}

public sealed class DefaultPieceColorRenderer : IPieceColorRenderer {
  public static DefaultPieceColorRenderer Instance { get; } = new();

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

//public sealed class GuardStonePieceColorRenderer : IPieceColorRenderer {
//  public static GuardStonePieceColorRenderer Instance { get; } = new();

//  readonly MaterialPropertyBlock _propertyBlock = new();

//  public void SetColors(List<Renderer> renderers, Color color, Color emissionColor) {
//    _propertyBlock.SetColor(ColorShaderId, color);
//    _propertyBlock.SetColor(EmissionColorShaderId, emissionColor);

//    foreach (Renderer renderer in renderers) {
//      renderer.SetPropertyBlock(_propertyBlock, 1);
//    }
//  }

//  public void ClearColors(List<Renderer> renderers) {
//    _propertyBlock.Clear();

//    foreach (Renderer renderer in renderers) {
//      renderer.SetPropertyBlock(null, 1);
//    }
//  }
//}

public sealed class PortalWoodPieceColorRenderer : IPieceColorRenderer {
  public static PortalWoodPieceColorRenderer Instance { get; } = new();

  public void SetColors(GameObject targetObject, Color color, Color emissionColor) {
    MaterialMan.s_instance.GetPropertyContainer(targetObject)
        .SetPropertyValue(ShaderProps._Color, color);
  }

  public void ClearColors(GameObject targetObject) {
    MaterialMan.s_instance.ResetValue(targetObject, ShaderProps._Color);
  }
}
