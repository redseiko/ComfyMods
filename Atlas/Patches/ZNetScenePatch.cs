namespace Atlas;

using HarmonyLib;

[HarmonyPatch(typeof(ZNetScene))]
static class ZNetScenePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNetScene.Awake))]
  static void AwakePostfix(ZNetScene __instance) {
    PrefabUtils.RegisterPrefabs(__instance.m_namedPrefabs);
  }
}
