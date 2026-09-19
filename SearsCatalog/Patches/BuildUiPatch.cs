namespace SearsCatalog;

using HarmonyLib;

[HarmonyPatch(typeof(BuildUi))]
static class BuildUiPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(BuildUi.Start))]
  static void StartPostfix(BuildUi __instance) {
    BuildUiController.SetupBuildUi(__instance);
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BuildUi.OnDestroy))]
  static void OnDestroyPrefix(BuildUi __instance) {
    BuildUiController.DestroyBuildUi(__instance);
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(BuildUi.OpenBuildMenu))]
  static void OpenBuildMenuPostfix(BuildUi __instance) {
    BuildUiController.ShowBuildUi(__instance);
  }
}
