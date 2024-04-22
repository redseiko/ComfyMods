namespace Unstrippable;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(ZDO))]
static class ZDOPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZDO.Strip), new[] { typeof(int) })]
  [HarmonyPatch(nameof(ZDO.Strip), new[] { typeof(int), typeof(long) })]
  [HarmonyPatch(nameof(ZDO.Strip), new[] { typeof(int), typeof(int) })]
  [HarmonyPatch(nameof(ZDO.Strip), new[] { typeof(int), typeof(Quaternion) })]
  [HarmonyPatch(nameof(ZDO.Strip), new[] { typeof(int), typeof(string) })]
  [HarmonyPatch(nameof(ZDO.Strip), new[] { typeof(int), typeof(byte[]) })]
  [HarmonyPatch(nameof(ZDO.StripConvert), new[] { typeof(ZDOID), typeof(int), typeof(long) })]
  [HarmonyPatch(nameof(ZDO.StripConvert), new[] { typeof(ZDOID), typeof(int), typeof(Vector3) })]
  [HarmonyPatch(nameof(ZDO.StripConvert), new[] { typeof(ZDOID), typeof(int), typeof(float) })]
  [HarmonyPatch(nameof(ZDO.StripLong), new[] { typeof(int) })]
  static bool StripPrefix(ref bool __result) {
    if (IsModEnabled.Value) {
      __result = false;
      return false;
    }

    return true;
  }
}
