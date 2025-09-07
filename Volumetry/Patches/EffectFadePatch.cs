namespace Volumetry;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(EffectFade))]
static class EffectFadePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(EffectFade.Awake))]
  static void AwakePostfix(EffectFade __instance) {
    if (IsModEnabled.Value) {
      VolumeController.ProcessEffectFade(__instance);
    }
  }
}
