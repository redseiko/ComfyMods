namespace ReportCard;

using ComfyLib;

using UnityEngine;

public static class PlayerStatsController {
  public static PlayerStatsPanel StatsPanel { get; private set; }

  public static bool IsStatsPanelValid() {
    return StatsPanel?.Panel;
  }

  public static void CreateStatsPanel(FejdStartup fejdStartup) {
    DestroyStatsPanel();
    StatsPanel = new(fejdStartup.m_characterSelectScreen.transform);

    StatsPanel.RectTransform
        .SetAnchorMin(new(1f, 0.5f))
        .SetAnchorMax(new(1f, 0.5f))
        .SetPivot(new(1f, 0.5f))
        .SetPosition(new(-25f, 0f))
        .SetSizeDelta(new(400f, 600f));

    StatsPanel.CloseButton.Button.onClick.AddListener(HideStatsPanel);

    HideStatsPanel();
  }

  public static void CreateStatsPanel(SkillsDialog skillsDialog) {
    DestroyStatsPanel();
    StatsPanel = new(skillsDialog.transform);

    StatsPanel.RectTransform
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(450f, 660f));

    StatsPanel.CloseButton.Button.onClick.AddListener(HideStatsPanel);

    HideStatsPanel();
  }

  public static void DestroyStatsPanel() {
    if (IsStatsPanelValid()) {
      UnityEngine.Object.Destroy(StatsPanel.Panel);
      StatsPanel = default;
    }
  }

  public static void ShowStatsPanel() {
    if (IsStatsPanelValid()) {
      StatsPanel.Panel.SetActive(true);
    }
  }

  public static void HideStatsPanel() {
    if (IsStatsPanelValid()) {
      StatsPanel.Panel.SetActive(false);
    }
  }

  public static void ToggleStatsPanel() {
    if (IsStatsPanelValid()) {
      StatsPanel.Panel.SetActive(!StatsPanel.Panel.activeSelf);
    }
  }

  public static void UpdateStatsPanel(PlayerProfile profile) {
    if (!IsStatsPanelValid()) {
      return;
    }

    if (profile?.m_playerStats == null) {
      ReportCard.LogInfo($"PlayerProfile ({profile?.GetName()}) has no valid PlayerStats.");
      HideStatsPanel();
    } else {
      StatsPanel.UpdateStatsList(profile);
    }
  }

  static LabelButton CreateStatsButton(Transform parentTransform) {
    LabelButton statsButton = new(parentTransform);
    statsButton.Container.name = "StatsButton";

    statsButton.Label
        .SetFontSize(20f)
        .SetText("Stats");

    return statsButton;
  }

  public static void CreateStatsButton(FejdStartup fejdStartup) {
    LabelButton statsButton = CreateStatsButton(fejdStartup.m_characterSelectScreen.transform);

    statsButton.Container.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.right)
        .SetAnchorMax(Vector2.right)
        .SetPivot(Vector2.right)
        .SetPosition(new(-25f, 20f))
        .SetSizeDelta(new(120f, 45f));

    statsButton.Button.onClick.AddListener(ToggleStatsPanel);
  }

  public static void CreateStatsButton(SkillsDialog skillsDialog) {
    LabelButton statsButton = CreateStatsButton(skillsDialog.transform.Find("SkillsFrame"));
    statsButton.Button.name = "StatsButton";

    statsButton.Container.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.one)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.one)
        .SetPosition(new(-25f, -2.5f))
        .SetSizeDelta(new(100f, 42.5f));

    statsButton.Button.onClick.AddListener(OnStatsButtonClick);
  }

  static void OnStatsButtonClick() {
    UpdateStatsPanel(Game.instance.Ref()?.m_playerProfile);
    ShowStatsPanel();
  }
}
