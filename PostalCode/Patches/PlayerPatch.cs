namespace PostalCode;

using HarmonyLib;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(Player.EdgeOfWorldKill))]
  static bool EdgeOfWorldKillPrefix() {
    return false;
  }
}
