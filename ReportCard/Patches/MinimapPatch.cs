namespace ReportCard;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Minimap))]
static class MinimapPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Minimap.Start))]
  static void StartPostfix(Minimap __instance) {
    if (IsModEnabled.Value) {
      ExploredStatsController.CreateStatsPanel(__instance);
      ExploredStatsController.CreateStatsButton(__instance);
      ExploredStatsController.ToggleStatsButton(ExploredStatsPanelShowStatsButton.Value);
    }
  }
}
