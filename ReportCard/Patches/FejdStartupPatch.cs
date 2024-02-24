namespace ReportCard;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.SetupGui))]
  static void SetupGuiPostfix(FejdStartup __instance) {
    if (IsModEnabled.Value) {
      PlayerStatsController.SetupPlayerStatsPanel(__instance.m_characterSelectScreen.transform);
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.SetupCharacterPreview))]
  static void SetupCharacterPreview(PlayerProfile profile) {
    if (IsModEnabled.Value) {
      PlayerStatsController.UpdatePlayerStatsPanel(profile);
    }
  }
}
