namespace DyeHard;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(VisEquipment))]
static class VisEquipmentPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(VisEquipment.SetHairColor))]
  static void SetHairColorPrefix(VisEquipment __instance, ref Vector3 color) {
    if (IsModEnabled.Value
        && OverridePlayerHairColor.Value
        && __instance == DyeManager.LocalVisEquipment) {
      color = DyeManager.GetPlayerHairColorVector();
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(VisEquipment.SetHairItem))]
  static void SetHairItemPrefix(VisEquipment __instance, ref string name) {
    if (IsModEnabled.Value
        && OverridePlayerHairItem.Value
        && __instance == DyeManager.LocalVisEquipment) {
      name = PlayerHairItem.Value;
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(VisEquipment.SetBeardItem))]
  static void SetBeardItemPrefix(VisEquipment __instance, ref string name) {
    if (IsModEnabled.Value
        && OverridePlayerBeardItem.Value
        && __instance == DyeManager.LocalVisEquipment) {
      name = PlayerBeardItem.Value;
    }
  }
}
