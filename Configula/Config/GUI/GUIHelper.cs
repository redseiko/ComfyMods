namespace Configula;

using System.Collections.Generic;

using UnityEngine;

public static class GUIHelper {
  static readonly Stack<Color> _colorStack = new();

  public static void BeginColor(Color color) {
    _colorStack.Push(GUI.color);
    GUI.color = color;
  }

  public static void EndColor() {
    GUI.color = _colorStack.Pop();
  }

  public static Texture2D CreateColorTexture(int width, int height, Color color) {
    Texture2D texture = new(width, height, TextureFormat.RGBA32, mipChain: false);

    for (int x = 0; x < width; x++) {
      for (int y = 0; y < height; y++) {
        texture.SetPixel(x, y, color);
      }
    }

    texture.Apply();

    return texture;
  }

  public static bool IsEnterPressed() {
    return
        Event.current.isKey
        && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
  }
}
