namespace ReportCard;

using ComfyLib;

using UnityEngine;

public static class ExploredStatsController {
  public static ExploredStatsPanel StatsPanel { get; private set; }

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
  }

  public static bool IsStatsPanelValid() {
    return StatsPanel?.Panel;
  }

  public static void ToggleStatsPanel() {
    if (IsStatsPanelValid()) {
      StatsPanel.Panel.SetActive(!StatsPanel.Panel.activeSelf);
    }
  }

  public static void HideStatsPanel() {
    if (IsStatsPanelValid()) {
      StatsPanel.Panel.SetActive(false);
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
        .SetAnchorMin(Vector2.one)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.one)
        .SetPosition(new(-25f, -20f))
        .SetSizeDelta(new(120f, 45f));

    StatsButton.Button.onClick.AddListener(ToggleStatsPanel);
  }
}
