namespace ReportCard;

using UnityEngine;

public static class PlayerStatsController {
  public static PlayerStatsPanel PlayerStatsPanel { get; private set; }

  public static void SetupPlayerStatsPanel(Transform parentTransform) {
    if (!PlayerStatsPanel?.Panel) {
      PlayerStatsPanel = new(parentTransform);
    }

    PlayerStatsPanel.HidePanel();
  }

  public static void UpdatePlayerStatsPanel(PlayerProfile profile) {
    PlayerStatsPanel?.UpdateStatsList(profile);
    PlayerStatsPanel?.ShowPanel();
  }
}
