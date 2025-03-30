namespace LetMePlay;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(PrivateArea))]
static class PrivateAreaPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(PrivateArea.RPC_FlashShield))]
  static bool PrivateAreaRpcFlashShield() {
    if (IsModEnabled.Value && DisableWardShieldFlash.Value) {
      return false;
    }

    return true;
  }
}
