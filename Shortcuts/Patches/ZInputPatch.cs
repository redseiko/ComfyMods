namespace Shortcuts;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ZInput))]
static class ZInputPatch {
  [HarmonyPostfix]
  [HarmonyPatch(MethodType.Constructor)]
  static void ConstructorPatch() {
    if (IsModEnabled.Value) {
      BindShortcutConfig(CurrentConfig);
    }
  }
}
