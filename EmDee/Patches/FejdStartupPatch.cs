namespace EmDee;

using HarmonyLib;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.Start))]
  static void StartPostfix(FejdStartup __instance) {
    EmDeeManager.Instance.CreateMarkdownPanel(__instance);
  }
}
