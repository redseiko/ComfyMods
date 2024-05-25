namespace ReportCard;

using ComfyLib;

using TMPro;

using UnityEngine;

public static class ExploredStatsController {
  public static ExploredStatsPanel StatsPanel { get; private set; }
  static readonly ExploredStats _exploredStats = new();

  public static void CreateStatsPanel(Minimap minimap) {
    StatsPanel = new(minimap.m_largeRoot.transform);

    StatsPanel.RectTransform
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(400f, 600f));

    StatsPanel.CloseButton.Button.onClick.AddListener(HideStatsPanel);

    HideStatsPanel();

    StatsPanel.Panel.AddComponent<MinimapFocusPanel>();
  }

  public static bool IsStatsPanelValid() {
    return StatsPanel?.Panel;
  }

  public static void ToggleStatsPanel() {
    if (IsStatsPanelValid()) {
      if (StatsPanel.Panel.activeSelf) {
        HideStatsPanel();
      } else {
        ShowStatsPanel();
      }
    }
  }

  public static void ShowStatsPanel() {
    if (IsStatsPanelValid()) {
      StatsPanel.Panel.SetActive(true);
      StatsPanel.ResetStatsList();

      TextMeshProUGUI label = StatsPanel.AddStatLabel();
      _exploredStats.Generate(Minimap.m_instance, label, UpdateStatsPanel);
    }
  }

  public static void HideStatsPanel() {
    if (IsStatsPanelValid()) {
      StatsPanel.Panel.SetActive(false);
    }
  }

  static void UpdateStatsPanel(ExploredStats explorerStats) {
    if (IsStatsPanelValid()) {
      StatsPanel.ResetStatsList();
      StatsPanel.UpdateStatsList(explorerStats);
    }
  }

  public static LabelButton StatsButton { get; private set; }

  public static void CreateStatsButton(Minimap minimap) {
    StatsButton = new(minimap.m_largeRoot.transform);
    StatsButton.Container.name = "StatsButton";

    StatsButton.Label
        .SetFontSize(20f)
        .SetText("Stats");

    StatsButton.Container.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.up)
        .SetPivot(Vector2.up)
        .SetPosition(new(25f, -20f))
        .SetSizeDelta(new(120f, 45f));

    StatsButton.Button.onClick.AddListener(ToggleStatsPanel);
  }
}
