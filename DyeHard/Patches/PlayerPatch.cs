namespace DyeHard;

using HarmonyLib;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.SetLocalPlayer))]
  static void SetLocalPlayerPostfix(Player __instance) {
    DyeManager.SetLocalPlayer(__instance);
    DyeManager.SetPlayerZDOHairColor();
    DyeManager.SetPlayerHairItem();
    DyeManager.SetPlayerBeardItem();
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.OnSpawned))]
  static void OnSpawnedPostfix(Player __instance) {
    DyeManager.SetLocalPlayer(__instance);
    DyeManager.SetPlayerZDOHairColor();
    DyeManager.SetPlayerHairItem();
    DyeManager.SetPlayerBeardItem();
  }
}
