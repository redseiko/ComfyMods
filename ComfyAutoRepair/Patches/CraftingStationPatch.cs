namespace ComfyAutoRepair;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(CraftingStation))]
static class CraftingStationPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(CraftingStation.Interact))]
  static void InteractPostfix(CraftingStation __instance, Humanoid user, bool repeat) {
    if (!repeat
        && IsModEnabled.Value
        && user == Player.m_localPlayer
        && __instance == Player.m_localPlayer.GetCurrentCraftingStation()) {
      RepairManager.RepairAllItems(Player.m_localPlayer, __instance, checkUsable: false);
    }
  }
}
