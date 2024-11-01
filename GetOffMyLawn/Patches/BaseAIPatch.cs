namespace GetOffMyLawn;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(BaseAI))]
static class BaseAIPatch {
  public static int TargetRayMask = 0;

  [HarmonyPostfix]
  [HarmonyPatch(nameof(BaseAI.Awake))]
  static void AwakePostfix() {
    if (IsModEnabled.Value && TargetRayMask == 0) {
      TargetRayMask = LayerMask.GetMask(["Default", "static_solid", "Default_small", "vehicle"]);

      BaseAI.m_monsterTargetRayMask = TargetRayMask;
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BaseAI.FindRandomStaticTarget))]
  static bool FindRandomStaticTargetPrefix(ref StaticTarget __result) {
    if (IsModEnabled.Value) {
      __result = null;
      return false;
    }

    return true;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BaseAI.FindClosestStaticPriorityTarget))]
  static bool FindClosestStaticPriorityTargetPrefix(ref StaticTarget __result) {
    if (IsModEnabled.Value) {
      __result = null;
      return false;
    }

    return true;
  }
}
