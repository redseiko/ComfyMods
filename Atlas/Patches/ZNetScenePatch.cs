namespace Atlas;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ZNetScene))]
static class ZNetScenePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNetScene.Awake))]
  static void AwakePostfix(ZNetScene __instance) {
    if (SetCustomFieldsForAshlandsZDOs.Value) {
      PrefabUtils.RegisterPrefabs(__instance.m_namedPrefabs);
    }
  }
}
