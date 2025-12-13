namespace ComfyLib;

using System;
using System.Collections.Generic;

using UnityEngine;

public static class UISpriteBuilder {
  public static readonly Color32 ColorWhite = Color.white;
  public static readonly Color32 ColorClear = Color.clear;

  static readonly Dictionary<string, Sprite> _spriteCache = [];

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

    Texture2D texture = new(width, height) {
      name = name,
      wrapMode = TextureWrapMode.Clamp,
      filterMode = filterMode,
    };

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
            new(borderWidth, borderHeight, borderWidth, borderHeight));

    sprite.name = name;
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
}
