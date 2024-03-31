namespace GetOffMyLawn;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(MonsterAI))]
static class MonsterAIPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(MonsterAI.UpdateTarget))]
  static void UpdateTargetPrefix(ref MonsterAI __instance) {
    if (IsModEnabled.Value) {
      __instance.m_attackPlayerObjects = false;
    }
  }
}
