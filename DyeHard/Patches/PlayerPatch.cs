namespace DyeHard;

using HarmonyLib;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.OnSpawned))]
  static void OnSpawnedPostfix(Player __instance) {
    DyeManager.SetLocalPlayer(__instance);
    DyeManager.SetPlayerZDOHairColor(__instance);
    DyeManager.SetPlayerHairItem(__instance);
    DyeManager.SetPlayerBeardItem(__instance);
  }
}
