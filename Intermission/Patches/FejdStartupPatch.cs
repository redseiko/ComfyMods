namespace Intermission;

using HarmonyLib;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.Awake))]
  static void AwakePostfix(FejdStartup __instance) {
    LoadingManager.SetupFejdStartup(__instance);
  }
}
