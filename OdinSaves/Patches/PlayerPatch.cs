namespace OdinSaves;

using HarmonyLib;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.OnDeath))]
  static void OnDeathPostfix(ref Player __instance) {
    Game.instance.m_playerProfile.ClearLoguoutPoint();
  }
}
