using System;

using UnityEngine;

namespace ComfyLib {
  public static class GUIBuilder {
    public static Texture2D CreateColorTexture(int width, int height, Color color) {
      Texture2D texture = new(width, height, TextureFormat.ARGB32, mipChain: false);
      texture.name =$"w-{width}-h-{height}-{color}-color";

      texture.wrapMode = TextureWrapMode.Clamp;
      texture.filterMode = FilterMode.Point;

      Color[] pixels = new Color[width * height];

      for (int i = 0; i < width * height; i++) {
        pixels[i] = color;
      }

      texture.SetPixels(pixels);
      texture.Apply();

      return texture;
    }
  }
}
