namespace ColorfulPieces;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Ship))]
static class ShipPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Ship.Awake))]
  static void AwakePostfix(Ship __instance) {
    if (IsModEnabled.Value) {
      __instance.gameObject.AddComponent<ShipColor>();
    }
  }
}
