namespace ReportCard;

using ComfyLib;

using UnityEngine;

public static class MahjongController {
  public static MahjongPanel MahjongPanel { get; private set; }
  public static MahjongHand CurrentHand { get; private set; } = new();

  public static bool IsPanelValid() => MahjongPanel?.Panel;

  public static void CreatePanel(FejdStartup fejdStartup) {
    MahjongTileResources.Initialize();

    DestroyPanel();
    MahjongPanel = new MahjongPanel(fejdStartup.m_characterSelectScreen.transform);

    MahjongPanel.RectTransform
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero);

    MahjongPanel.CloseButton.Button.onClick.AddListener(HidePanel);
    MahjongPanel.OnDiscardRequested += DiscardTile;

    CurrentHand.Initialize();
    BuildHandView();

    HidePanel();
  }

  public static void DiscardTile(MahjongTileInfo tileInfo) {
    if (!IsPanelValid()) {
      return;
    }
    
    CurrentHand.Discard(tileInfo);   
    CurrentHand.Draw(MahjongTileHelper.GetRandomTileInfo());
    
    BuildHandView();
  }

  public static void BuildHandView() {
    if (!IsPanelValid()) {
      return;
    }

    MahjongPanel.ClearHand();

    foreach (MahjongTileInfo tileInfo in CurrentHand.Tiles) {
      MahjongPanel.AddTileToHand(tileInfo);
    }

    MahjongPanel.SetIncomingTile(CurrentHand.IncomingTile);
  }

  public static void DestroyPanel() {
    if (IsPanelValid()) {
      Object.Destroy(MahjongPanel.Panel);
      MahjongPanel = null;
    }
  }

  public static void ShowPanel() {
    if (IsPanelValid()) {
      MahjongPanel.Panel.SetActive(true);
    }
  }

  public static void HidePanel() {
    if (IsPanelValid()) {
      MahjongPanel.Panel.SetActive(false);
    }
  }

  public static void TogglePanel() {
    if (IsPanelValid()) {
      MahjongPanel.Panel.SetActive(!MahjongPanel.Panel.activeSelf);
    }
  }

  static LabelButton CreateMahjongButton(Transform parentTransform) {
    LabelButton button = new(parentTransform);
    button.Container.name = "MahjongButton";

    button.Label
        .SetFontSize(20f)
        .SetText("Mahjong");

    return button;
  }

  public static void CreateMahjongButton(FejdStartup fejdStartup) {
    LabelButton button = CreateMahjongButton(fejdStartup.m_characterSelectScreen.transform);

    button.RectTransform
        .SetAnchorMin(Vector2.right)
        .SetAnchorMax(Vector2.right)
        .SetPivot(Vector2.right)
        .SetPosition(new(-25f, 80f))
        .SetSizeDelta(new(120f, 45f));

    button.Button.onClick.AddListener(TogglePanel);
  }
}
