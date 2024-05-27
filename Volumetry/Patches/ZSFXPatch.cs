namespace Volumetry;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ZSFX))]
static class ZSFXPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZSFX.Play))]
  static void PlayPostfix(ZSFX __instance) {
    if (IsModEnabled.Value) {
      VolumeController.ProcessSFX(__instance);
    }
  }
}
