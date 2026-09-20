namespace Pseudonym;

using HarmonyLib;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.Start))]
  static void StartPostfix(FejdStartup __instance) {
    FejdStartupManager.SetupCharacterSelect(__instance);
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.UpdateCharacterList))]
  static void UpdateCharacterListPostfix(FejdStartup __instance) {
    FejdStartupManager.UpdateCharacterList(__instance);
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(FejdStartup.OnNewCharacterCancel))]
  static void OnNewCharacterCancelPrefix(FejdStartup __instance) {
    FejdStartupManager.OnNewCharacterCancel(__instance);
  }
}
