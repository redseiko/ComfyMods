namespace SkyTree;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(EnvMan))]
static class EnvManPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(EnvMan.Awake))]
  static void AwakePostfix(EnvMan __instance) {
    if (IsModEnabled.Value) {
      YggdrasilManager.SetYggdrasilSolid(__instance);
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(EnvMan.OnDestroy))]
  static void OnDestroyPrefix() {
    YggdrasilManager.Reset();
  }
}
