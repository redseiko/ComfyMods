namespace Contextual;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.SetupGui))]
  static void SetupGuiPostfix(FejdStartup __instance) {
    if (IsModEnabled.Value) {
      __instance.gameObject.AddComponent<ContextMenuController>();
    }
  }
}
