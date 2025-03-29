namespace Dramamist;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Demister))]
static class DemisterPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Demister.OnEnable))]
  static void OnEnablePostfix(Demister __instance) {
    if (IsModEnabled.Value) {
      ParticleMistManager.UpdateDemisterSettings(__instance);
    }
  }
}
