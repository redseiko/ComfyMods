namespace ReportCard;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;

public static class MahjongController {
  public static MahjongPanel MahjongPanel { get; private set; }
  public static List<MahjongTileInfo> PlayerHand { get; } = [];
  public static MahjongTileInfo IncomingTile { get; private set; }

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

    PlayerHand.Clear();

    for (int i = 0; i < 13; i++) {
      PlayerHand.Add(MahjongTileHelper.GetRandomTileInfo());
    }

    IncomingTile = MahjongTileHelper.GetRandomTileInfo();
    BuildHandView();

    HidePanel();
  }

  public static void BuildHandView() {
    if (!IsPanelValid()) {
      return;
    }

    MahjongPanel.ClearHand();

    foreach (MahjongTileInfo tileInfo in PlayerHand) {
      MahjongPanel.AddTileToHand(tileInfo);
    }

    MahjongPanel.SetIncomingTile(IncomingTile);
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
