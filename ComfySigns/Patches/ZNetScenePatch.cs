using HarmonyLib;

namespace ComfySigns {
  [HarmonyPatch(typeof(ZNetScene))]
  static class ZNetScenePatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZNetScene.Awake))]
    static void AwakePostfix(ZNetScene __instance) {
      if (PluginConfig.IsModEnabled.Value) {
        ComfySigns.SetupSignPrefabs(__instance);
      }
    }
  }
}
