namespace PotteryBarn;

using HarmonyLib;

[HarmonyPatch(typeof(ArmorStand))]
static class ArmorStandPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ArmorStand.CanAttach))]
  static void CanAttachPostfix(ArmorStand __instance, ItemDrop.ItemData item, ref bool __result) {
    if (__result && !PotteryManager.CanAttachItemToArmorStand(__instance, item)) {
      __result = false;
    }
  }
}
