using System;

using UnityEngine;

namespace ComfyLib {
  public static class GUIBuilder {
    public static Texture2D CreateColorTexture(int width, int height, Color color) {
      return CreateColorTexture(width, height, color, 0, color);
    }

    public static Texture2D CreateColorTexture(int width, int height, Color color, int radius, Color outsideColor) {
      if (width <= 0 || height <= 0) {
        throw new ArgumentException("Texture width and height must be > 0.");
      }

      if (radius < 0 || radius > width || radius > height) {
        throw new ArgumentException("Texture radius must be >= 0 and < width/height.");
      }

      Texture2D texture = new(width, height, TextureFormat.ARGB32, mipChain: false);
      texture.name =$"w-{width}-h-{height}-rad-{radius}-color-{ColorId(color)}-ocolor-{ColorId(outsideColor)}";
      texture.wrapMode = TextureWrapMode.Clamp;
      texture.filterMode = FilterMode.Trilinear;

      Color[] pixels = new Color[width * height];

      for (int y = 0; y < height; y++) {
        for (int x = 0; x < width; x++) {
          pixels[(y * width) + x] = IsCornerPixel(x, y, width, height, radius) ? outsideColor : color;
        }
      }

      texture.SetPixels(pixels);
      texture.Apply();

      return texture;
    }

    static string ColorId(Color color) {
      return $"{color.r:F3}r-{color.g:F3}g-{color.b:F3}b-{color.a:F3}a";
    }

    static bool IsCornerPixel(int x, int y, int w, int h, int rad) {
      if (rad == 0) {
        return false;
      }

      int dx = Math.Min(x, w - x);
      int dy = Math.Min(y, h - y);

      if (dx == 0 && dy == 0) {
        return true;
      }

      if (dx > rad || dy > rad) {
        return false;
      }

      dx = rad - dx;
      dy = rad - dy;

      return Math.Round(Math.Sqrt(dx * dx + dy * dy)) > rad;
    }
  }
}
