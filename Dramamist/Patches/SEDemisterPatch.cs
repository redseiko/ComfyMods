namespace Dramamist;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(SE_Demister))]
static class SEDemisterPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(SE_Demister.UpdateStatusEffect))]
  static void UpdateStatusEffectPrefix(SE_Demister __instance, ref bool __state) {
    if (IsModEnabled.Value) {
      __state = __instance.m_ballInstance;
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(SE_Demister.UpdateStatusEffect))]
  static void UpdateStatusEffectPostfix(SE_Demister __instance, ref bool __state) {
    if (IsModEnabled.Value
        && __instance.m_ballInstance
        && !__state
        && __instance.m_ballInstance.TryGetComponentInChildren(out Demister demister)) {
      demister.GetOrAddComponent<FadeOutParticleMist>();
    }
  }
}
