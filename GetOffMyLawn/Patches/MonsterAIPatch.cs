namespace GetOffMyLawn;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(MonsterAI))]
static class MonsterAIPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(MonsterAI.Awake))]
  static void AwakePostfix(ref MonsterAI __instance) {
    if (IsModEnabled.Value) {
      __instance.m_attackPlayerObjects = false;
    }
  }
}
