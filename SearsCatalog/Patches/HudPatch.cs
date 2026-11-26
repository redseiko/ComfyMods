namespace SearsCatalog;

using HarmonyLib;

[HarmonyPatch(typeof(Hud))]
static class HudPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(Hud.OnDestroy))]
  static void OnDestroyPrefix(Hud __instance) {
    BuildUiController.DestroyBuildUi(__instance.m_buildUi);
  }
}
