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
        DrawCharacters(pixels, info.Rank);
        break;

      case MahjongSuit.Winds:
        DrawWinds(pixels, info.Rank);
        break;

      case MahjongSuit.Dragons:
        DrawDragons(pixels, info.Rank);
        break;
    }
  }

  static void DrawCharacters(Color32[] pixels, int rank) {
    // Top: Blue Number
    // Bottom: Red "Wan"

    byte[,] numMap = rank switch {
      1 => PixelMaps.Num1,
      2 => PixelMaps.Num2,
      3 => PixelMaps.Num3,
      4 => PixelMaps.Num4,
      5 => PixelMaps.Num5,
      6 => PixelMaps.Num6,
      7 => PixelMaps.Num7,
      8 => PixelMaps.Num8,
      9 => PixelMaps.Num9,
      _ => PixelMaps.Num1
    };

    // Draw Number (Centered Top)
    // Map is 10x10
    DrawPixelMap(pixels, 23, 8, numMap, ColorBlue);

    // Draw "Wan" (Centered Bottom)
    // Map is 12x12. Scale 2x => 24x24
    DrawPixelMap(pixels, 16, 32, PixelMaps.CharWan, ColorRed, 2);
  }

  static void DrawWinds(Color32[] pixels, int rank) {
    byte[,] map = rank switch {
      1 => PixelMaps.WindE,
      2 => PixelMaps.WindS,
      3 => PixelMaps.WindW,
      4 => PixelMaps.WindN,
      _ => PixelMaps.WindE
    };

    // Map is 10x10. Scale 3x => 30x30. Centered approx (56-30)/2 = 13
    DrawPixelMap(pixels, 13, 17, map, ColorBlack, 3);
  }

  static void DrawDragons(Color32[] pixels, int rank) {
    switch (rank) {
      case 1: // Red (Chung/Center)
        DrawPixelMap(pixels, 10, 14, PixelMaps.DragonC, ColorRed, 3);
        break;
      case 2: // Green (Fa/Prosperity)
        DrawPixelMap(pixels, 10, 14, PixelMaps.DragonF, ColorGreen, 3);
        break;
      case 3: // White (Po/Board)
        DrawRect(pixels, new Rect(10, 8, 36, 48), ColorBlue);
        DrawRect(pixels, new Rect(14, 12, 28, 40), ColorClear);
        break;
    }
  }

  static void DrawDots(Color32[] pixels, int rank) {
    // ... (Existing DrawDots logic is fine, kept in file) ...
    float cx = FaceWidth / 2f;
    float cy = FaceHeight / 2f;
    float r = 5.5f;

    switch (rank) {
      case 1:
        DrawCircle(pixels, new Vector2(cx, cy), 16f, ColorRed);
        DrawCircle(pixels, new Vector2(cx, cy), 8f, ColorRed);
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
        DrawCircle(pixels, new Vector2(cx - 12, cy), r, ColorRed);
        DrawDotColumn(pixels, cx + 12, cy, 14, r, ColorGreen);
        DrawCircle(pixels, new Vector2(cx + 12, cy), r, ColorRed);
        break;

      case 7:
        DrawCircle(pixels, new Vector2(cx - 12, cy - 16), r, ColorGreen);
        DrawCircle(pixels, new Vector2(cx, cy - 12), r, ColorGreen);
        DrawCircle(pixels, new Vector2(cx + 12, cy - 8), r, ColorGreen);
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

            if (col == 0) {
              c = ColorRed;
            } else if (col == 1) {
              c = ColorGreen;
            }

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
    // Kept as is
    if (rank == 1) {
      DrawRect(pixels, new Rect(15, 20, 26, 24), ColorGreen);
      return;
    }

    float w = 6f;
    float h = 18f;
    float gap = 2f;

    for (int i = 0; i < rank; i++) {
      float x = 5 + (i % 3) * (w + gap * 4);
      float y = 5 + (i / 3) * (h + gap);
      DrawRect(pixels, new Rect(x, y, w, h), ColorGreen);
    }
  }

  static void DrawPixelMap(Color32[] pixels, int startX, int startY, byte[,] map, Color32 color, int scale = 1) {
    int height = map.GetLength(0);
    int width = map.GetLength(1);

    for (int y = 0; y < height; y++) {
      for (int x = 0; x < width; x++) {
        if (map[y, x] == 1) {
          if (scale == 1) {
            int px = startX + x;
            int py = startY + y;

            if (px >= 0 && px < FaceWidth && py >= 0 && py < FaceHeight) {
              pixels[py * FaceWidth + px] = color;
            }
          } else {
            DrawRect(pixels, new Rect(startX + (x * scale), startY + (y * scale), scale, scale), color);
          }
        }
      }
    }
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
    int xMin = Mathf.Max(0, (int) rect.x);
    int xMax = Mathf.Min(FaceWidth, (int) rect.xMax);
    int yMin = Mathf.Max(0, (int) rect.y);
    int yMax = Mathf.Min(FaceHeight, (int) rect.yMax);

    for (int y = yMin; y < yMax; y++) {
      for (int x = xMin; x < xMax; x++) {
        pixels[y * FaceWidth + x] = color;
      }
    }
  }

  static class PixelMaps {
    public static readonly byte[,] Num1 = {
      {0,0,0,0,1,1,0,0,0,0},
      {0,0,0,1,1,1,0,0,0,0},
      {0,0,0,0,1,1,0,0,0,0},
      {0,0,0,0,1,1,0,0,0,0},
      {0,0,0,0,1,1,0,0,0,0},
      {0,0,0,0,1,1,0,0,0,0},
      {0,0,0,0,1,1,0,0,0,0},
      {0,0,0,1,1,1,1,0,0,0},
      {0,0,0,1,1,1,1,0,0,0},
      {0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly byte[,] Num2 = {
      {0,0,1,1,1,1,1,0,0,0}, {0,1,1,0,0,0,1,1,0,0}, {0,0,0,0,0,0,1,1,0,0}, {0,0,0,0,0,1,1,0,0,0},
      {0,0,0,0,1,1,0,0,0,0}, {0,0,0,1,1,0,0,0,0,0}, {0,0,1,1,0,0,0,0,0,0}, {0,1,1,1,1,1,1,1,0,0},
      {0,1,1,1,1,1,1,1,0,0}, {0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly byte[,] Num3 = {
      {0,0,1,1,1,1,1,0,0,0}, {0,1,1,0,0,0,1,1,0,0}, {0,0,0,0,0,0,1,1,0,0}, {0,0,0,0,1,1,1,0,0,0},
      {0,0,0,0,0,0,1,1,0,0}, {0,0,0,0,0,0,1,1,0,0}, {0,1,1,0,0,0,1,1,0,0}, {0,0,1,1,1,1,1,0,0,0},
      {0,0,0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly byte[,] Num4 = {
      {0,0,0,0,0,1,1,0,0,0}, {0,0,0,0,1,1,1,0,0,0}, {0,0,0,1,1,1,1,0,0,0}, {0,0,1,1,0,1,1,0,0,0},
      {0,1,1,0,0,1,1,0,0,0}, {0,1,1,1,1,1,1,1,1,0}, {0,1,1,1,1,1,1,1,1,0}, {0,0,0,0,0,1,1,0,0,0},
      {0,0,0,0,0,1,1,0,0,0}, {0,0,0,0,0,1,1,0,0,0}
    };

    public static readonly byte[,] Num5 = {
      {0,1,1,1,1,1,1,1,0,0}, {0,1,1,0,0,0,0,0,0,0}, {0,1,1,1,1,1,0,0,0,0}, {0,0,0,0,0,1,1,1,0,0},
      {0,0,0,0,0,0,1,1,0,0}, {0,0,0,0,0,0,1,1,0,0}, {0,1,1,0,0,0,1,1,0,0}, {0,0,1,1,1,1,1,0,0,0},
      {0,0,0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly byte[,] Num6 = {
      {0,0,0,1,1,1,1,0,0,0}, {0,0,1,1,0,0,0,0,0,0}, {0,1,1,0,0,0,0,0,0,0}, {0,1,1,1,1,1,1,0,0,0},
      {0,1,1,0,0,0,1,1,0,0}, {0,1,1,0,0,0,1,1,0,0}, {0,1,1,0,0,0,1,1,0,0}, {0,0,1,1,1,1,1,0,0,0},
      {0,0,0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly byte[,] Num7 = {
      {0,1,1,1,1,1,1,1,0,0}, {0,1,1,0,0,0,1,1,0,0}, {0,0,0,0,0,1,1,0,0,0}, {0,0,0,0,1,1,0,0,0,0},
      {0,0,0,1,1,0,0,0,0,0}, {0,0,0,1,1,0,0,0,0,0}, {0,0,1,1,0,0,0,0,0,0}, {0,0,1,1,0,0,0,0,0,0},
      {0,0,1,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly byte[,] Num8 = {
      {0,0,1,1,1,1,1,0,0,0}, {0,1,1,0,0,0,1,1,0,0}, {0,1,1,0,0,0,1,1,0,0}, {0,0,1,1,1,1,1,0,0,0},
      {0,1,1,0,0,0,1,1,0,0}, {0,1,1,0,0,0,1,1,0,0}, {0,1,1,0,0,0,1,1,0,0}, {0,0,1,1,1,1,1,0,0,0},
      {0,0,0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly byte[,] Num9 = {
      {0,0,1,1,1,1,1,0,0,0}, {0,1,1,0,0,0,1,1,0,0}, {0,1,1,0,0,0,1,1,0,0}, {0,0,1,1,1,1,1,1,0,0},
      {0,0,0,0,0,0,1,1,0,0}, {0,0,0,0,0,0,1,1,0,0}, {0,0,0,0,0,1,1,0,0,0}, {0,0,1,1,1,1,0,0,0,0},
      {0,0,0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly byte[,] CharWan = {
        {0,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,1,0,0,1,1,0,0,1,1,0}, // Grass top
        {1,1,1,1,1,1,1,1,1,1,1,1},
        {0,0,0,1,1,0,0,1,1,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,1,1,1,1,1,1,1,1,1,0}, // Roof/Box
        {0,1,0,0,1,1,0,0,1,0,1,0},
        {0,1,0,0,1,1,0,0,1,0,1,0},
        {0,1,1,1,1,1,1,1,1,1,1,0},
        {0,0,1,0,1,0,0,1,0,1,0,0}, // Legs (simplified)
        {0,1,0,0,1,0,0,1,0,0,1,0},
        {0,1,0,0,1,0,0,1,0,0,1,0}
    };

    public static readonly byte[,] WindE = {
      {1,1,1,1,1,1,1,1,1,0},
      {1,0,0,0,0,0,0,0,0,0},
      {1,0,0,0,0,0,0,0,0,0},
      {1,0,0,0,0,0,0,0,0,0},
      {1,1,1,1,1,1,1,0,0,0},
      {1,0,0,0,0,0,0,0,0,0},
      {1,0,0,0,0,0,0,0,0,0},
      {1,0,0,0,0,0,0,0,0,0},
      {1,1,1,1,1,1,1,1,1,0},
      {0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly byte[,] WindS = {
      {0,1,1,1,1,1,1,1,0,0},
      {1,1,0,0,0,0,0,0,0,0},
      {1,1,0,0,0,0,0,0,0,0},
      {0,1,1,1,1,1,1,0,0,0},
      {0,0,0,0,0,0,1,1,0,0},
      {0,0,0,0,0,0,0,1,1,0},
      {0,0,0,0,0,0,0,1,1,0},
      {1,1,0,0,0,0,0,1,1,0},
      {0,1,1,1,1,1,1,1,0,0},
      {0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly byte[,] WindW = {
      {1,0,0,0,0,0,0,0,1,0},
      {1,0,0,0,0,0,0,0,1,0},
      {1,0,0,0,0,0,0,0,1,0},
      {1,1,0,0,0,0,0,1,1,0},
      {1,1,0,0,1,0,0,1,1,0},
      {1,1,0,0,1,0,0,1,1,0},
      {1,0,1,1,0,1,1,0,1,0},
      {1,0,1,1,0,1,1,0,1,0},
      {1,0,1,1,0,1,1,0,1,0},
      {0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly byte[,] WindN = {
      {1,1,0,0,0,0,0,1,1,0},
      {1,1,1,0,0,0,0,1,1,0},
      {1,1,1,1,0,0,0,1,1,0},
      {1,1,0,1,1,0,0,1,1,0},
      {1,1,0,0,1,1,0,1,1,0},
      {1,1,0,0,0,1,1,1,1,0},
      {1,1,0,0,0,0,1,1,1,0},
      {1,1,0,0,0,0,0,1,1,0},
      {1,1,0,0,0,0,0,1,1,0},
      {0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly byte[,] DragonC = {
        {0,0,1,1,1,1,1,1,0,0,0,0},
        {0,1,1,0,0,0,0,1,1,0,0,0},
        {1,1,0,0,0,0,0,0,1,1,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0},
        {1,1,0,0,0,0,0,0,1,1,0,0},
        {0,1,1,0,0,0,0,1,1,0,0,0},
        {0,0,1,1,1,1,1,1,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0}
    };

    public static readonly byte[,] DragonF = {
        {1,1,1,1,1,1,1,1,1,1,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0},
        {1,1,1,1,1,1,1,0,0,0,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0},
        {1,1,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0}
    };
  }
}
