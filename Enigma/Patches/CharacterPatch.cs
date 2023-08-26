using HarmonyLib;

using System;

using static Enigma.Enigma;
using static Enigma.PluginConfig;

namespace Enigma.Patches {
  [HarmonyPatch(typeof(Character))]
  public class CharacterPatch {

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Character.GetHoverName))]
    public static bool GetHoverNamePrefix(ref Character __instance, ref string __result) {
      if (!IsModEnabled.Value
          || __instance == null 
          || !__instance.TryGetComponent(out ZNetView zNetView) 
          || zNetView.GetZDO() == null) {

        return true;
      }

      string customName = zNetView.GetZDO().GetString(CustomNameFieldName);

      if (!string.IsNullOrEmpty(customName)) {
        __result = customName;
        return false;
      }

      return true;
    }


    [HarmonyPrefix]
    [HarmonyPatch(nameof(Character.IsBoss))]
    public static bool IsBossPrefix(ref Character __instance, ref bool __result) {
      if (!IsModEnabled.Value
          || __instance == null
          || !__instance.TryGetComponent(out ZNetView zNetView) 
          || zNetView.GetZDO() == null) {

        return true;
      }

     
      if (zNetView.GetZDO().GetBool(BossDesignationFieldName, false) && !string.IsNullOrEmpty(zNetView.GetZDO().GetString(CustomNameFieldName))) {
        __result = true;
        return false;
      }
      
      return true;
    }
  }
}
