namespace ReportCard;

using System.Collections.Generic;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class MahjongPanel {
  public GameObject Panel { get; }
  public RectTransform RectTransform { get; }
  public LabelButton CloseButton { get; }
  public GameObject HandContainer { get; }
  public List<MahjongTile> HandTiles { get; } = [];

  public MahjongPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();
    CloseButton = CreateCloseButton(RectTransform);
    HandContainer = CreateHandContainer(RectTransform);
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = UIBuilder.CreatePanel(parentTransform);
    panel.name = "MahjongPanel";

    panel.GetComponent<RectTransform>()
        .SetSizeDelta(new(1020f, 600f));

    return panel;
  }

  static GameObject CreateHandContainer(Transform parentTransform) {
    GameObject container = new("HandContainer", typeof(RectTransform));
    container.transform.SetParent(parentTransform, false);

    container.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(970f, 100f));

    return container;
  }

  public void AddTile(MahjongTileInfo info) {
    MahjongTile tile = new(HandContainer.transform);
    tile.SetTile(info);
    HandTiles.Add(tile);
    RedrawHand();
  }

  public void RemoveTile(MahjongTile tile) {
    if (HandTiles.Remove(tile)) {
      Object.Destroy(tile.Container);
      RedrawHand();
    }
  }

  void RedrawHand() {
    if (HandTiles.Count == 0) {
      return;
    }

    const float tileWidth = MahjongTile.TileWidth;
    const float spacing = 5f;

    float totalHandWidth = (HandTiles.Count * tileWidth) + (Mathf.Max(0, HandTiles.Count - 1) * spacing);
    float startX = -totalHandWidth / 2f + tileWidth / 2f;

    for (int i = 0; i < HandTiles.Count; i++) {
      HandTiles[i].RectTransform.anchoredPosition = new Vector2(startX + i * (tileWidth + spacing), 0);
    }
  }

  static LabelButton CreateCloseButton(Transform parentTransform) {
    LabelButton closeButton = new(parentTransform);
    closeButton.Button.name = "CloseButton";

    closeButton.RectTransform
        .SetAnchorMin(Vector2.right)
        .SetAnchorMax(Vector2.right)
        .SetPivot(Vector2.right)
        .SetPosition(new(-20f, 20f))
        .SetSizeDelta(new(100f, 42.5f));

    closeButton.Label
        .SetFontSize(18f)
        .SetText("Close");

    return closeButton;
  }
}
