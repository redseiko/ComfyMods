namespace ColorfulPieces;

using System.Collections.Generic;

using UnityEngine;

using static PluginConstants;

public interface IPieceColorRenderer {
  void SetColors(List<Renderer> renderers, Color color, Color emissionColor);
  void ClearColors(List<Renderer> renderers);
}

public sealed class DefaultPieceColorRenderer : IPieceColorRenderer {
  public static DefaultPieceColorRenderer Instance { get; } = new();

  readonly MaterialPropertyBlock _propertyBlock = new();

  public void SetColors(List<Renderer> renderers, Color color, Color emissionColor) {
    _propertyBlock.SetColor(ColorShaderId, color);
    _propertyBlock.SetColor(EmissionColorShaderId, emissionColor);

    foreach (Renderer renderer in renderers) {
      renderer.SetPropertyBlock(_propertyBlock);
    }
  }

  public void ClearColors(List<Renderer> renderers) {
    _propertyBlock.Clear();

    foreach (Renderer renderer in renderers) {
      renderer.SetPropertyBlock(null);
    }
  }
}

public sealed class GuardStonePieceColorRenderer : IPieceColorRenderer {
  public static GuardStonePieceColorRenderer Instance { get; } = new();

  readonly MaterialPropertyBlock _propertyBlock = new();

  public void SetColors(List<Renderer> renderers, Color color, Color emissionColor) {
    _propertyBlock.SetColor(ColorShaderId, color);
    _propertyBlock.SetColor(EmissionColorShaderId, emissionColor);

    foreach (Renderer renderer in renderers) {
      renderer.SetPropertyBlock(_propertyBlock, 1);
    }
  }

  public void ClearColors(List<Renderer> renderers) {
    _propertyBlock.Clear();

    foreach (Renderer renderer in renderers) {
      renderer.SetPropertyBlock(null, 1);
    }
  }
}

public sealed class PortalWoodPieceColorRenderer : IPieceColorRenderer {
  public static PortalWoodPieceColorRenderer Instance { get; } = new();

  readonly MaterialPropertyBlock _propertyBlock = new();

  public void SetColors(List<Renderer> renderers, Color color, Color emissionColor) {
    _propertyBlock.SetColor(ColorShaderId, color);

    foreach (Renderer renderer in renderers) {
      renderer.SetPropertyBlock(_propertyBlock);
    }
  }

  public void ClearColors(List<Renderer> renderers) {
    _propertyBlock.Clear();

    foreach (Renderer renderer in renderers) {
      renderer.SetPropertyBlock(null);
    }
  }
}
