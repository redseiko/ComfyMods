namespace Volumetry;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ZSFX))]
static class ZSFXPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZSFX.Play))]
  static void PlayPostfix(ZSFX __instance) {
    if (IsModEnabled.Value && __instance.m_audioSource && __instance.m_audioSource.clip) {
      VolumeController.ProcessSfx(__instance);
    }
  }
}
