namespace ReportCard;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

public static class MahjongTileResources {
  private const int TileWidth = 70;
  private const int TileHeight = 80;
  private const int TileCornerRadius = 5;

  public static Sprite TileSprite { get; private set; }

  public static readonly ColorBlock TileColors = new() {
    normalColor = new Color32(245, 245, 220, 255),
    highlightedColor = new Color32(255, 255, 224, 255),
    pressedColor = new Color32(222, 184, 135, 255),
    selectedColor = new Color32(255, 255, 224, 255),
    disabledColor = new Color32(211, 211, 211, 150),
    colorMultiplier = 1f,
    fadeDuration = 0.1f
  };

  public static void Initialize() {
    if (!TileSprite) {
      TileSprite = UISpriteBuilder.CreateRoundedCornerSprite(TileWidth, TileHeight, TileCornerRadius);
    }
  }
}
