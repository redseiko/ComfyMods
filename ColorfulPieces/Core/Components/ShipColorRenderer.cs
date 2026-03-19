namespace ColorfulPieces;

using UnityEngine;

public interface IShipColorRenderer {
  void SetColors(GameObject targetObject, Color color, Color emissionColor);
  void ClearColors(GameObject targetObject);
}

public sealed class DefaultShipColorRenderer : IShipColorRenderer {
  public static DefaultShipColorRenderer Instance { get; } = new();

  static readonly MaterialPropertyBlock _matBlock = new();

  public void SetColors(GameObject targetObject, Color color, Color emissionColor) {
    if (targetObject.name.StartsWith("Raft")) {
      ApplyColorsToPaths(
          targetObject,
          color,
          emissionColor,
          "ship/visual/hull_new",
          "ship/visual/mast",
          "ship/visual/mast/Sail/sail_full",
          "interactive/controls/rudder/Rudder");
    } else {
      MaterialMan.s_instance.GetPropertyContainer(targetObject)
          .SetPropertyValue(ShaderProps._Color, color)
          .SetPropertyValue(ShaderProps._EmissionColor, emissionColor);
    }
  }

  public void ClearColors(GameObject targetObject) {
    if (targetObject.name.StartsWith("Raft")) {
      ClearColorsFromPaths(
          targetObject,
          "ship/visual/hull_new",
          "ship/visual/mast",
          "ship/visual/mast/Sail/sail_full",
          "interactive/controls/rudder/Rudder");
    } else {
      MaterialMan.s_instance.ResetValue(targetObject, ShaderProps._Color);
      MaterialMan.s_instance.ResetValue(targetObject, ShaderProps._EmissionColor);
    }
  }

  static void ApplyColorsToPaths(GameObject root, Color color, Color emissionColor, params string[] paths) {
    foreach (string path in paths) {
      Transform transform = root.transform.Find(path);

      if (!transform) {
        continue;
      }

      foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>()) {
        renderer.GetPropertyBlock(_matBlock);
        _matBlock.SetColor(ShaderProps._Color, color);
        _matBlock.SetColor(ShaderProps._EmissionColor, emissionColor);
        renderer.SetPropertyBlock(_matBlock);
      }
    }
  }

  static void ClearColorsFromPaths(GameObject root, params string[] paths) {
    foreach (string path in paths) {
      Transform transform = root.transform.Find(path);

      if (!transform) {
        continue;
      }

      foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>()) {
        renderer.SetPropertyBlock(null);
      }
    }
  }
}
