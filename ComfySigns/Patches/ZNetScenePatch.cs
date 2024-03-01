namespace ComfySigns;

using HarmonyLib;

[HarmonyPatch(typeof(ZNetScene))]
static class ZNetScenePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNetScene.Awake))]
  static void AwakePostfix(ZNetScene __instance) {
    if (PluginConfig.IsModEnabled.Value) {
      SignUtils.SetupSignPrefabs(__instance);
    }
  }
}
