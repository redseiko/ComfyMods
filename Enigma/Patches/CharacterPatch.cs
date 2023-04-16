using HarmonyLib;

using System;

using static Enigma.Enigma;

namespace Entitlement.Patches {
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
  }
}
