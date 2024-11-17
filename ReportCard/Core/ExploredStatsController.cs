namespace ReportCard;

using ComfyLib;

using TMPro;

using UnityEngine;

public static class ExploredStatsController {
  public static ExploredStatsPanel StatsPanel { get; private set; }
  static readonly ExploredStats _exploredStats = new();

  public static bool IsStatsPanelValid() {
    return StatsPanel?.Panel;
  }

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
      StatsPanel.UpdateStatus(string.Empty);

      _exploredStats.Generate(Minimap.m_instance, StatsPanel.StatusLabel, UpdateStatsPanel);
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
      StatsPanel.UpdateStatus(GenerateStatusString(Player.m_localPlayer, ZNet.m_world, EnvMan.s_instance));
    }
  }

  static string GenerateStatusString(Player player, World world, EnvMan envManager) {
    if (!player || world == default || !envManager) {
      return string.Empty;
    }

    return
        $"<align=left>{player.GetPlayerName()}<line-height=0>\n"
            + $"<align=right>{world.m_name}<line-height=1em>\n"
            + $"<align=left><color=#D3D3D3>{player.GetPlayerID()}</color><line-height=0>\n"
            + $"<align=right>Day <color=#FFD600>{envManager.GetCurrentDay()}</color><line-height=1em>";
  }

  public static LabelButton StatsButton { get; private set; }

  public static bool IsStatsButtonValid() {
    return StatsButton?.Container;
  }

  public static void ToggleStatsButton(bool toggleOn) {
    if (IsStatsButtonValid()) {
      StatsButton.Container.SetActive(toggleOn);
    }
  }

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
        .SetSizeDelta(new(100f, 42.5f));

    StatsButton.Button.onClick.AddListener(ToggleStatsPanel);
  }
}
