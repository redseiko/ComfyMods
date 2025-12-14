namespace ReportCard;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

public sealed class MahjongPanel {
  const float TileSpacing = 5f;
  const float TileSelectionYOffset = 30f;

  public GameObject Panel { get; }
  public RectTransform RectTransform { get; }
  public LabelButton CloseButton { get; }
  public GameObject PlayerHandArea { get; }
  public List<MahjongTile> PlayerHandTiles { get; } = [];
  public GameObject IncomingTileArea { get; }
  MahjongTile _selectedTile;

  public MahjongPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();
    CloseButton = CreateCloseButton(RectTransform);
    PlayerHandArea = CreatePlayerHandArea(RectTransform);
    IncomingTileArea = CreateIncomingTileArea(RectTransform);

    Button backgroundButton = Panel.AddComponent<Button>();
    backgroundButton.targetGraphic = Panel.GetComponent<Image>();
    backgroundButton.transition = Selectable.Transition.None;
    backgroundButton.onClick.AddListener(HandleBackgroundClicked);
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = UIBuilder.CreatePanel(parentTransform);
    panel.name = "MahjongPanel";

    panel.GetComponent<RectTransform>()
        .SetSizeDelta(new(1400f, 600f));

    return panel;
  }

  static GameObject CreatePlayerHandArea(Transform parentTransform) {
    GameObject container = new("PlayerHandArea", typeof(RectTransform));
    container.transform.SetParent(parentTransform, false);

    container.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(new(0, -100f))
        .SetSizeDelta(new(970f, 100f));

    return container;
  }

  static GameObject CreateIncomingTileArea(Transform parentTransform) {
    GameObject container = new("IncomingTileArea", typeof(RectTransform));
    container.transform.SetParent(parentTransform, false);

    container.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(new(540f, -100f))
        .SetSizeDelta(new(MahjongTile.TileWidth, 100f));

    return container;
  }

  readonly List<MahjongTile> _tilesToRemove = [];

  public void ClearHand() {
    _tilesToRemove.AddRange(PlayerHandTiles);

    foreach (MahjongTile tile in _tilesToRemove) {
      RemoveTile(tile);
    }

    _tilesToRemove.Clear();

    foreach (Transform child in IncomingTileArea.transform) {
      Object.Destroy(child.gameObject);
    }
  }

  public void AddTileToHand(MahjongTileInfo info) {
    MahjongTile tile = new(PlayerHandArea.transform);
    tile.SetTile(info);
    tile.OnTileClicked += HandleTileClicked;
    PlayerHandTiles.Add(tile);
    UpdateHandLayout();
  }

  public void SetIncomingTile(MahjongTileInfo info) {
    foreach (Transform child in IncomingTileArea.transform) {
      Object.Destroy(child.gameObject);
    }

    MahjongTile tile = new(IncomingTileArea.transform);
    tile.SetTile(info);
  }

  public void RemoveTile(MahjongTile tile) {
    if (PlayerHandTiles.Remove(tile)) {
      tile.OnTileClicked -= HandleTileClicked;
      Object.Destroy(tile.Container);
      UpdateHandLayout();
    }
  }

  void HandleTileClicked(MahjongTile tile) {
    if (_selectedTile == null) {
      _selectedTile = tile;
      Vector2 currentPos = _selectedTile.RectTransform.anchoredPosition;
      _selectedTile.RectTransform.anchoredPosition = new Vector2(currentPos.x, currentPos.y + TileSelectionYOffset);
    } else if (_selectedTile == tile) {
      MahjongController.DiscardTile(_selectedTile.Info);
      _selectedTile = null;
    } else {
      Vector2 currentPos = _selectedTile.RectTransform.anchoredPosition;
      _selectedTile.RectTransform.anchoredPosition = new Vector2(currentPos.x, currentPos.y - TileSelectionYOffset);

      _selectedTile = tile;

      currentPos = _selectedTile.RectTransform.anchoredPosition;
      _selectedTile.RectTransform.anchoredPosition = new Vector2(currentPos.x, currentPos.y + TileSelectionYOffset);
    }
  }

  void HandleBackgroundClicked() {
    if (_selectedTile != null) {
      Vector2 currentPos = _selectedTile.RectTransform.anchoredPosition;
      _selectedTile.RectTransform.anchoredPosition = new Vector2(currentPos.x, currentPos.y - TileSelectionYOffset);
      _selectedTile = null;
    }
  }

  void UpdateHandLayout() {
    float totalWidth = PlayerHandTiles.Count * MahjongTile.TileWidth + (PlayerHandTiles.Count - 1) * TileSpacing;
    float startX = -totalWidth / 2f + MahjongTile.TileWidth / 2f;

    for (int i = 0; i < PlayerHandTiles.Count; i++) {
      MahjongTile tile = PlayerHandTiles[i];
      float xPos = startX + i * (MahjongTile.TileWidth + TileSpacing);
      tile.RectTransform.anchoredPosition = new Vector2(xPos, 0);
      tile.RectTransform.SetSiblingIndex(i);
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
