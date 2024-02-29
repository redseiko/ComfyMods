namespace Volumetry;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ZSFX))]
static class ZSFXPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZSFX.Play))]
  static void PlayPostfix(ZSFX __instance) {
    if (IsModEnabled.Value) {
      // Do the thing on the __instance.
    }
  }
}
