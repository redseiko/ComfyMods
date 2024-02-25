namespace ReportCard;

using ComfyLib;

using UnityEngine;

public static class PlayerStatsController {
  public static PlayerStatsPanel StatsPanel { get; private set; }

  public static void CreateStatsPanel(FejdStartup fejdStartup) {
    DestroyStatsPanel();
    StatsPanel = new(fejdStartup.m_characterSelectScreen.transform);

    StatsPanel.Panel.GetComponent<RectTransform>()
        .SetAnchorMin(new(1f, 0.5f))
        .SetAnchorMax(new(1f, 0.5f))
        .SetPivot(new(1f, 0.5f))
        .SetPosition(new(-25f, 0f))
        .SetSizeDelta(new(400f, 600f));

    StatsPanel.HidePanel();
  }

  public static void CreateStatsPanel(SkillsDialog skillsDialog) {
    DestroyStatsPanel();
    StatsPanel = new(skillsDialog.transform);

    StatsPanel.Panel.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(400f, 625f));

    StatsPanel.HidePanel();
  }

  public static void DestroyStatsPanel() {
    if (StatsPanel?.Panel) {
      UnityEngine.Object.Destroy(StatsPanel.Panel);
      StatsPanel = default;
    }
  }

  public static void HideStatsPanel() {
    if (StatsPanel?.Panel) {
      StatsPanel.HidePanel();
    }
  }

  public static void UpdateStatsPanel(PlayerProfile profile) {
    StatsPanel.UpdateStatsList(profile);
    StatsPanel.ShowPanel();
  }

  public static void CreateStatsButton(Transform parentTransform) {
    ButtonCell statsButton = new(parentTransform);
    statsButton.Button.name = "StatsButton";

    statsButton.Cell.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.one)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.one)
        .SetPosition(new(-25f, -2.5f))
        .SetSizeDelta(new(100f, 42.5f));

    statsButton.Label
        .SetFontSize(20f)
        .SetText("Stats");

    statsButton.Button.onClick.AddListener(OnStatsButtonClick);
  }

  static void OnStatsButtonClick() {
    UpdateStatsPanel(Game.instance.m_playerProfile);
  }
}
