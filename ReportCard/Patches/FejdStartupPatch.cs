namespace ReportCard;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.SetupGui))]
  static void SetupGuiPostfix(FejdStartup __instance) {
    if (IsModEnabled.Value) {
      PlayerStatsController.CreateStatsPanel(__instance);
      PlayerStatsController.CreateStatsButton(__instance);
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.ShowCharacterSelection))]
  static void ShowCharacterSelectionPostfix() {
    if (IsModEnabled.Value && OpenStatsPanelOnCharacterSelect.Value) {
      PlayerStatsController.ShowStatsPanel();
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.SetupCharacterPreview))]
  static void SetupCharacterPreview(PlayerProfile profile) {
    if (IsModEnabled.Value) {
      PlayerStatsController.UpdateStatsPanel(profile);
    }
  }
}
