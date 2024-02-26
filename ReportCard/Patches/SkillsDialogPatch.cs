namespace ReportCard;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(SkillsDialog))]
static class SkillsDialogPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(SkillsDialog.Awake))]
  static void AwakePostfix(SkillsDialog __instance) {
    if (IsModEnabled.Value) {
      PlayerStatsController.CreateStatsPanel(__instance);
      PlayerStatsController.CreateStatsButton(__instance);
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(SkillsDialog.OnClose))]
  static void OnClosePostfix() {
    if (IsModEnabled.Value) {
      PlayerStatsController.HideStatsPanel();
    }
  }
}
