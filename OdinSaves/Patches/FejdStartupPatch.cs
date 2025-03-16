namespace OdinSaves;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.Awake))]
  static void AwakePostfix(FejdStartup __instance) {
    if (IsModEnabled.Value && EnableMapDataCompression.Value) {
      SaveManager.CreateProfileCompressionUI(__instance);
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.UpdateCharacterList))]
  static void UpdateCharacterListPostfix(FejdStartup __instance) {
    if (IsModEnabled.Value && EnableMapDataCompression.Value) {
      SaveManager.UpdateProfileCompressionUI(__instance);
    }
  }
}
