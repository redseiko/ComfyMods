using HarmonyLib;

using System;

using static Enigma.Enigma;

namespace Enigma.Patches {
  [HarmonyPatch(typeof(Character))]
  public class CharacterPatch {

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Character.GetHoverName))]
    public static bool GetHoverNamePrefix(ref Character __instance, ref string __result) {
      if (__instance.TryGetComponent(out ZNetView zNetView)) {
        string customName = zNetView.GetZDO().GetString(CustomNameFieldName);
        if (!string.IsNullOrEmpty(customName)) {
          __result = customName;
          return false;
        }
      }
      return true;
    }


    [HarmonyPrefix]
    [HarmonyPatch(nameof(Character.IsBoss))]
    public static bool IsBossPrefix(ref Character __instance, ref bool __result) {
      if (__instance.TryGetComponent(out ZNetView zNetView)) {
        if (zNetView.GetZDO().GetBool(BossDesignationFieldName, false) && !string.IsNullOrEmpty(zNetView.GetZDO().GetString(CustomNameFieldName))) {
          __result = true;
          return false;
        }
      }
      return true;
    }
  }
}
