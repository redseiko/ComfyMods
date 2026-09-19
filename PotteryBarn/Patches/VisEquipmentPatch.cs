namespace PotteryBarn;

using System.Collections.Generic;

using HarmonyLib;

using MagicaCloth2;

using static PluginConfig;

[HarmonyPatch(typeof(VisEquipment))]
static class VisEquipmentPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(VisEquipment.SetupCloth))]
  static void SetupClothPrefix(VisEquipment __instance, ref List<ColliderComponent> __state) {
    if (__instance.m_isArmorStand && IsModEnabled.Value) {
      __state = __instance.m_clothColliders;
      __instance.m_clothColliders = [];
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(VisEquipment.SetupCloth))]
  static void SetupClothPostfix(VisEquipment __instance, List<ColliderComponent> __state) {
    if (__instance.m_isArmorStand && IsModEnabled.Value) {
      __instance.m_clothColliders = __state;
    }
  }
}
