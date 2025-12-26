namespace ReportCard;

using System.Collections.Generic;
using UnityEngine;

public static class MahjongTileTextureGenerator {
  static readonly Dictionary<string, Sprite> _cache = [];

  const int FaceWidth = 56;
  const int FaceHeight = 64;

  static readonly Color32 ColorRed = new(200, 0, 0, 255);
  static readonly Color32 ColorGreen = new(0, 180, 0, 255);
  static readonly Color32 ColorBlue = new(0, 0, 200, 255);
  static readonly Color32 ColorBlack = new(0, 0, 0, 255);
  static readonly Color32 ColorClear = new(0, 0, 0, 0);

  public static Sprite GetTileSprite(MahjongTileInfo info) {
    string spriteName = $"{info.Suit}-{info.Rank}";

    if (_cache.TryGetValue(spriteName, out Sprite cachedSprite)) {
      return cachedSprite;
    }

    Texture2D texture = new(FaceWidth, FaceHeight) {
      name = spriteName,
      filterMode = FilterMode.Point,
      wrapMode = TextureWrapMode.Clamp
    };

    Color32[] pixels = new Color32[FaceWidth * FaceHeight];

    for (int i = 0; i < pixels.Length; i++) {
      pixels[i] = ColorClear;
    }

    DrawTileContent(pixels, info);

    texture.SetPixels32(pixels);
    texture.Apply();

    Sprite sprite = Sprite.Create(
        texture,
        new Rect(0, 0, FaceWidth, FaceHeight),
        new Vector2(0.5f, 0.5f),
        100f
    );

    sprite.name = spriteName;
    _cache[spriteName] = sprite;

    return sprite;
  }

  static void DrawTileContent(Color32[] pixels, MahjongTileInfo info) {
    switch (info.Suit) {
      case MahjongSuit.Dots:
        DrawDots(pixels, info.Rank);
        break;

      case MahjongSuit.Bamboos:
        DrawBamboo(pixels, info.Rank);
        break;

      case MahjongSuit.Characters:
      case MahjongSuit.Winds:
      case MahjongSuit.Dragons:
        DrawPlaceholder(pixels, info);
        break;
    }
  }

  static void DrawDots(Color32[] pixels, int rank) {
    float cx = FaceWidth / 2f;
    float cy = FaceHeight / 2f;
    float r = 5.5f;

    switch (rank) {
      case 1:
        DrawCircle(pixels, new Vector2(cx, cy), 16f, ColorRed);
        DrawCircle(pixels, new Vector2(cx, cy), 8f, ColorRed); // Inner circle detail
        return;

      case 2:
        DrawCircle(pixels, new Vector2(cx, cy - 14), r, ColorBlue);
        DrawCircle(pixels, new Vector2(cx, cy + 14), r, ColorBlue);
        break;

      case 3:
        DrawCircle(pixels, new Vector2(cx - 14, cy - 14), r, ColorBlue);
        DrawCircle(pixels, new Vector2(cx, cy), r, ColorRed);
        DrawCircle(pixels, new Vector2(cx + 14, cy + 14), r, ColorBlue);
        break;

      case 4:
        DrawDotColumn(pixels, cx - 12, cy, 14, r, ColorBlue);
        DrawDotColumn(pixels, cx + 12, cy, 14, r, ColorGreen);
        break;

      case 5:
        DrawDotColumn(pixels, cx - 12, cy, 14, r, ColorBlue);
        DrawDotColumn(pixels, cx + 12, cy, 14, r, ColorGreen);
        DrawCircle(pixels, new Vector2(cx, cy), r, ColorRed);
        break;

      case 6:
        DrawDotColumn(pixels, cx - 12, cy, 14, r, ColorGreen);
        DrawCircle(pixels, new Vector2(cx - 12, cy), r, ColorRed); // Middle red
        DrawDotColumn(pixels, cx + 12, cy, 14, r, ColorGreen);
        DrawCircle(pixels, new Vector2(cx + 12, cy), r, ColorRed); // Middle red
        break;

      case 7:
        // Top 3 diagonal-ish
        DrawCircle(pixels, new Vector2(cx - 12, cy - 16), r, ColorGreen);
        DrawCircle(pixels, new Vector2(cx, cy - 12), r, ColorGreen);
        DrawCircle(pixels, new Vector2(cx + 12, cy - 8), r, ColorGreen);
        
        // Bottom 4
        DrawCircle(pixels, new Vector2(cx - 12, cy + 8), r, ColorRed);
        DrawCircle(pixels, new Vector2(cx - 12, cy + 22), r, ColorRed);
        DrawCircle(pixels, new Vector2(cx + 12, cy + 8), r, ColorRed);
        DrawCircle(pixels, new Vector2(cx + 12, cy + 22), r, ColorRed);
        break;

      case 8:
        for (int i = 0; i < 4; i++) {
          float y = cy - 21 + (i * 14);
          DrawCircle(pixels, new Vector2(cx - 12, y), r, ColorBlue);
          DrawCircle(pixels, new Vector2(cx + 12, y), r, ColorBlue);
        }
        break;

      case 9:
        for (int col = -1; col <= 1; col++) {
          for (int row = -1; row <= 1; row++) {
            Color32 c = ColorBlue;
            if (col == 0) c = ColorRed;
            else if (col == 1) c = ColorGreen;

            DrawCircle(pixels, new Vector2(cx + (col * 16), cy + (row * 16)), r, c);
          }
        }
        break;
    }
  }

  static void DrawDotColumn(Color32[] pixels, float x, float cy, float spacing, float r, Color32 color) {
    DrawCircle(pixels, new Vector2(x, cy - spacing), r, color);
    DrawCircle(pixels, new Vector2(x, cy + spacing), r, color);
  }

  static void DrawBamboo(Color32[] pixels, int rank) {
    if (rank == 1) {
      // Draw a "Bird" (Green Rect)
      DrawRect(pixels, new Rect(15, 20, 26, 24), ColorGreen);
      return;
    }

    // Draw sticks
    float w = 6f;
    float h = 18f;
    float gap = 2f;

    // Simply stack them for now
    for (int i = 0; i < rank; i++) {
      float x = 5 + (i % 3) * (w + gap * 4);
      float y = 5 + (i / 3) * (h + gap);
      DrawRect(pixels, new Rect(x, y, w, h), ColorGreen);
    }
  }

  static void DrawPlaceholder(Color32[] pixels, MahjongTileInfo info) {
    // Draw a border to indicate it's a character/honor
    DrawRect(pixels, new Rect(5, 5, 46, 54), ColorRed);
    DrawRect(pixels, new Rect(8, 8, 40, 48), ColorClear);
  }

  static void DrawCircle(Color32[] pixels, Vector2 center, float radius, Color32 color) {
    int xMin = Mathf.Max(0, Mathf.FloorToInt(center.x - radius));
    int xMax = Mathf.Min(FaceWidth, Mathf.CeilToInt(center.x + radius));
    int yMin = Mathf.Max(0, Mathf.FloorToInt(center.y - radius));
    int yMax = Mathf.Min(FaceHeight, Mathf.CeilToInt(center.y + radius));

    float rSq = radius * radius;

    for (int y = yMin; y < yMax; y++) {
      for (int x = xMin; x < xMax; x++) {
        float dx = x - center.x;
        float dy = y - center.y;

        if (dx * dx + dy * dy <= rSq) {
          pixels[y * FaceWidth + x] = color;
        }
      }
    }
  }

  static void DrawRect(Color32[] pixels, Rect rect, Color32 color) {
    int xMin = Mathf.Max(0, (int)rect.x);
    int xMax = Mathf.Min(FaceWidth, (int)rect.xMax);
    int yMin = Mathf.Max(0, (int)rect.y);
    int yMax = Mathf.Min(FaceHeight, (int)rect.yMax);

    for (int y = yMin; y < yMax; y++) {
      for (int x = xMin; x < xMax; x++) {
        pixels[y * FaceWidth + x] = color;
      }
    }
  }
}
