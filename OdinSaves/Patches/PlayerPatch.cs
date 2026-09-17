namespace OdinSaves;

using HarmonyLib;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.OnDeath))]
  static void OnDeathPostfix(Player __instance) {
    if (__instance == Player.m_localPlayer) {
      Game.instance.m_playerProfile.ClearLoguoutPoint();
    }
  }
}
