namespace ComfySigns;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ZNetScene))]
static class ZNetScenePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNetScene.Awake))]
  static void AwakePostfix(ZNetScene __instance) {
    if (IsModEnabled.Value) {
      SignUtils.SetupSignPrefabs(__instance);
    }
  }
}
