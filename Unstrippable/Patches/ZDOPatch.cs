namespace Unstrippable;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(ZDO))]
static class ZDOPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZDO.Strip), [typeof(int)])]
  [HarmonyPatch(nameof(ZDO.Strip), [typeof(int), typeof(long)])]
  [HarmonyPatch(nameof(ZDO.Strip), [typeof(int), typeof(int)])]
  [HarmonyPatch(nameof(ZDO.Strip), [typeof(int), typeof(Quaternion)])]
  [HarmonyPatch(nameof(ZDO.Strip), [typeof(int), typeof(string)])]
  [HarmonyPatch(nameof(ZDO.Strip), [typeof(int), typeof(byte[])])]
  [HarmonyPatch(nameof(ZDO.StripConvert), [typeof(ZDOID), typeof(int), typeof(long)])]
  [HarmonyPatch(nameof(ZDO.StripConvert), [typeof(ZDOID), typeof(int), typeof(Vector3)])]
  [HarmonyPatch(nameof(ZDO.StripLong), [typeof(int)])]
  static bool StripPrefix(ref bool __result) {
    if (IsModEnabled.Value) {
      __result = false;
      return false;
    }

    return true;
  }
}
