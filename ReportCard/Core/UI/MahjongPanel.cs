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
  public MahjongTile IncomingTile { get; private set; }

  public Button BackgroundButton { get; }

  MahjongTile _selectedTile;

  public MahjongPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();
    CloseButton = CreateCloseButton(RectTransform);
    PlayerHandArea = CreatePlayerHandArea(RectTransform);
    IncomingTileArea = CreateIncomingTileArea(RectTransform);

    IncomingTileArea = CreateIncomingTileArea(RectTransform);
    BackgroundButton = CreateBackgroundButton(Panel, HandleBackgroundClicked);
  }

  static Button CreateBackgroundButton(GameObject panel, UnityEngine.Events.UnityAction onClick) {
    Button backgroundButton = panel.AddComponent<Button>();
    backgroundButton.targetGraphic = panel.GetComponent<Image>();
    backgroundButton.transition = Selectable.Transition.None;
    backgroundButton.onClick.AddListener(onClick);

    return backgroundButton;
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

  public void ClearHand() {
    for (int i = 0; i < PlayerHandTiles.Count; i++) {
      if (PlayerHandTiles[i] != null && PlayerHandTiles[i].Container) {
        Object.Destroy(PlayerHandTiles[i].Container);
      }
    }

    PlayerHandTiles.Clear();

    if (IncomingTile != null) {
      if (IncomingTile.Container) {
        Object.Destroy(IncomingTile.Container);
      }

      IncomingTile = null;
    }
  }

  public void AddTileToHand(MahjongTileInfo info) {
    MahjongTile tile = new(PlayerHandArea.transform);
    tile.SetTile(info);
    tile.OnTileClicked += HandleTileClicked;

    PlayerHandTiles.Add(tile);
    UpdateHandLayout();
  }

  public void AnimateDraw(MahjongTileInfo info) {
    IncomingTile = new(IncomingTileArea.transform);
    IncomingTile.SetTile(info);
    IncomingTile.OnTileClicked += HandleTileClicked;

    IncomingTile.AnimateSpawn(new Vector2(200f, 0f), Vector2.zero, null);
  }

  public void AnimateDiscard(MahjongTile tile) {
    if (tile == IncomingTile) {
      tile.OnTileClicked -= HandleTileClicked;

      tile.AnimateDiscard(() => {
        if (tile != null && tile.Container) {
            Object.Destroy(tile.Container);
        }
        if (IncomingTile == tile) {
            IncomingTile = null;
        }
      });

      return;
    }

    if (PlayerHandTiles.Remove(tile)) {
      tile.OnTileClicked -= HandleTileClicked;

      tile.AnimateDiscard(() => {
        Object.Destroy(tile.Container);
      });

      UpdateHandLayout();
    }
  }

  public void AnimateSort() {
    if (IncomingTile != null) {
      IncomingTile.RectTransform.SetParent(PlayerHandArea.transform, true);

      IncomingTile.OnTileClicked = null;
      IncomingTile.OnTileClicked += HandleTileClicked;
      
      IncomingTile.RectTransform.TweenKill();

      PlayerHandTiles.Add(IncomingTile);
      IncomingTile = null;
    }

    PlayerHandTiles.Sort((a, b) => {
      int indexA = MahjongController.CurrentHand.HandTiles.IndexOf(a.Info);
      int indexB = MahjongController.CurrentHand.HandTiles.IndexOf(b.Info);

      return indexA.CompareTo(indexB);
    });

    UpdateHandLayout();
  }

  public void RemoveTileFromHand(MahjongTile tile) {
    if (PlayerHandTiles.Remove(tile)) {
      tile.OnTileClicked -= HandleTileClicked;
      Object.Destroy(tile.Container);

      UpdateHandLayout();
    }
  }

  public event System.Action<MahjongTileInfo> OnDiscardRequested;

  void HandleTileClicked(MahjongTile tile) {
    if (_selectedTile == null) {
      _selectedTile = tile;
      Vector2 currentPos = _selectedTile.RectTransform.anchoredPosition;

      _selectedTile.RectTransform.TweenKill();
      _selectedTile.RectTransform
          .TweenAnchoredPosition(new Vector2(currentPos.x, currentPos.y + TileSelectionYOffset), 0.2f)
          .SetEase(Ease.OutBack)
          .TweenStart();
    } else if (_selectedTile == tile) {
      OnDiscardRequested?.Invoke(_selectedTile.Info);
      _selectedTile = null;
    } else {
      Vector2 currentPos = _selectedTile.RectTransform.anchoredPosition;

      _selectedTile.RectTransform.TweenKill();
      _selectedTile.RectTransform
          .TweenAnchoredPosition(new Vector2(currentPos.x, currentPos.y - TileSelectionYOffset), 0.2f)
          .SetEase(Ease.OutQuad)
          .TweenStart();

      _selectedTile = tile;

      currentPos = _selectedTile.RectTransform.anchoredPosition;

      _selectedTile.RectTransform.TweenKill();
      _selectedTile.RectTransform
          .TweenAnchoredPosition(new Vector2(currentPos.x, currentPos.y + TileSelectionYOffset), 0.2f)
          .SetEase(Ease.OutBack)
          .TweenStart();
    }
  }

  void HandleBackgroundClicked() {
    if (_selectedTile != null) {
      Vector2 currentPosition = _selectedTile.RectTransform.anchoredPosition;

      _selectedTile.RectTransform
          .TweenAnchoredPosition(new Vector2(currentPosition.x, currentPosition.y - TileSelectionYOffset), 0.2f)
          .SetEase(Ease.OutQuad)
          .TweenStart();

      _selectedTile = null;
    }
  }

  void UpdateHandLayout() {
    float totalWidth = PlayerHandTiles.Count * MahjongTile.TileWidth + (PlayerHandTiles.Count - 1) * TileSpacing;
    float startX = -totalWidth / 2f + MahjongTile.TileWidth / 2f;

    for (int i = 0; i < PlayerHandTiles.Count; i++) {
      MahjongTile tile = PlayerHandTiles[i];
      float xPos = startX + i * (MahjongTile.TileWidth + TileSpacing);

      tile.RectTransform.TweenKill();
      tile.RectTransform
          .TweenAnchoredPosition(new Vector2(xPos, 0), 0.2f)
          .SetEase(Ease.OutQuad)
          .TweenStart();

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
