namespace GetOffMyLawn;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(BaseAI))]
static class BaseAIPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(BaseAI.Awake))]
  static IEnumerable<CodeInstruction> AwakeTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(
                OpCodes.Call, AccessTools.Method(typeof(LayerMask), nameof(LayerMask.GetMask), [typeof(string[])])),
            new CodeMatch(OpCodes.Stsfld, AccessTools.Field(typeof(BaseAI), nameof(BaseAI.m_monsterTargetRayMask))))
        .ThrowIfInvalid($"Could not patch BaseAI.Awake()! (monster-target-ray-mask)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(BaseAIPatch), nameof(GetMonsterTargetRayMaskDelegate))))
        .InstructionEnumeration();
  }

  static int GetMonsterTargetRayMaskDelegate(int value) {
    if (IsModEnabled.Value) {
      return LayerMask.GetMask(["Default", "static_solid", "Default_small", "vehicle"]);
    }

    return value;
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
