namespace BetterZeeLog;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(FejdStartup.UpdateKeyboard))]
  static bool UpdateKeyboardPrefix() {
    if (IsModEnabled.Value && Console.IsVisible()) {
      return false;
    }

    return true;
  }
}
