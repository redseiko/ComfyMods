using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace ComfyLib {
  public static class UIBuilder {
    static readonly Dictionary<string, Sprite> _spriteCache = [];

    public static readonly Color32 ColorWhite = Color.white;
    public static readonly Color32 ColorClear = Color.clear;

    public static Sprite CreateRect(int width, int height, Color32 color) {
      string name = $"Rectangle-{width}w-{height}h-{color}c";

      if (_spriteCache.TryGetValue(name, out Sprite sprite)) {
        return sprite;
      }

      Texture2D texture = new(width, height) {
        name = name,
        wrapMode = TextureWrapMode.Repeat,
        filterMode = FilterMode.Point
      };

      Color32[] pixels = new Color32[width * height];

      for (int i = 0; i < pixels.Length; i++) {
        pixels[i] = color;
      }

      texture.SetPixels32(pixels);
      texture.Apply();

      sprite =
          Sprite.Create(
              texture,
              new(0, 0, width, height),
              new(0.5f, 0.5f),
              pixelsPerUnit: 100f,
              extrude: 0,
              SpriteMeshType.FullRect);

      sprite.name = name;
      _spriteCache[name] = sprite;

      return sprite;
    }

    public static Sprite CreateRoundedCornerSprite(
        int width, int height, int radius, FilterMode filterMode = FilterMode.Bilinear) {
      string name = $"RoundedCorner-{width}w-{height}h-{radius}r";

      if (_spriteCache.TryGetValue(name, out Sprite sprite)) {
        return sprite;
      }

      Texture2D texture =
          new Texture2D(width, height)
              .SetName(name)
              .SetWrapMode(TextureWrapMode.Clamp)
              .SetFilterMode(filterMode);

      Color32[] pixels = new Color32[width * height];

      for (int y = 0; y < height; y++) {
        for (int x = 0; x < width; x++) {
          pixels[(y * width) + x] = IsCornerPixel(x, y, width, height, radius) ? ColorClear : ColorWhite;
        }
      }

      texture.SetPixels32(pixels);
      texture.Apply();

      int borderWidth;
      for (borderWidth = 0; borderWidth < width; borderWidth++) {
        if (pixels[borderWidth] == Color.white) {
          break;
        }
      }

      int borderHeight;
      for (borderHeight = 0; borderHeight < height; borderHeight++) {
        if (pixels[borderHeight * width] == Color.white) {
          break;
        }
      }

      sprite =
          Sprite.Create(
              texture,
              new(0, 0, width, height),
              new(0.5f, 0.5f),
              pixelsPerUnit: 100f,
              extrude: 0,
              SpriteMeshType.FullRect,
              new(borderWidth, borderHeight, borderWidth, borderHeight))
          .SetName(name);

      _spriteCache[name] = sprite;
      return sprite;
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

    public static Sprite CreateSuperellipse(int width, int height, float exponent) {
      string name = $"Superellipse-{width}w-{height}h-{exponent}e";

      if (_spriteCache.TryGetValue(name, out Sprite sprite)) {
        return sprite;
      }

      Texture2D texture = new(width, height) {
        name = name,
        wrapMode = TextureWrapMode.Clamp,
        filterMode = FilterMode.Point
      };

      Color32[] pixels = new Color32[width * height];

      int XYToIndex(int x, int y) {
        return x + (y * width);
      }

      int mx = width / 2;
      int my = height / 2;

      float factor = 1f;
      float a = factor * (width / 2f);
      float b = factor * (height / 2f);

      for (int x = 0; x < mx; x++) {
        for (int y = 0; y < my; y++) {
          float lhs = Mathf.Pow(Mathf.Abs(x / a), exponent) + Mathf.Pow(Mathf.Abs(y / b), exponent);
          Color32 color = lhs > 1f ? ColorClear : ColorWhite;

          int rightx = x + mx;
          int leftx = -x + mx - 1;
          int topy = -y + my - 1;
          int bottomy = y + my;

          pixels[XYToIndex(rightx, bottomy)] = color;
          pixels[XYToIndex(rightx, topy)] = color;
          pixels[XYToIndex(leftx, topy)] = color;
          pixels[XYToIndex(leftx, bottomy)] = color;
        }
      }

      texture.SetPixels32(pixels);
      texture.Apply();

      int borderWidth;
      for (borderWidth = 0; borderWidth < width; borderWidth++) {
        if (pixels[borderWidth] == Color.white) {
          break;
        }
      }

      int borderHeight;
      for (borderHeight = 0; borderHeight < height; borderHeight++) {
        if (pixels[borderHeight * width] == Color.white) {
          break;
        }
      }

      sprite =
          Sprite.Create(
              texture,
              new(0, 0, width, height),
              new(0.5f, 0.5f),
              pixelsPerUnit: 100f,
              extrude: 0,
              SpriteMeshType.FullRect,
              new(borderWidth, borderHeight, borderWidth, borderHeight));

      sprite.name = name;
      _spriteCache[name] = sprite;

      return sprite;
    }

    public static TextMeshProUGUI CreateLabel(Transform parentTransform) {
      TextMeshProUGUI label =
          UnityEngine.Object.Instantiate(UnifiedPopup.instance.bodyText, parentTransform, worldPositionStays: false);

      label.name = "Label";
      label.fontSize = 16f;
      label.enableAutoSizing = false;
      label.richText = true;
      label.color = Color.white;
      label.text = string.Empty;

      return label;
    }
  }
}