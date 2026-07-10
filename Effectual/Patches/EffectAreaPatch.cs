namespace Effectual;

using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(EffectArea))]
static class EffectAreaPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(EffectArea.GetBaseValue))]
  static IEnumerable<CodeInstruction> GetBaseValueTranspiler(IEnumerable<CodeInstruction> instructions) {
    if (!IsModEnabled.Value) {
      return instructions;
    }

    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldsfld),
            new CodeMatch(OpCodes.Call, AccessTools.Method(
                typeof(UnityEngine.Physics),
                nameof(UnityEngine.Physics.OverlapSphereNonAlloc),
                [typeof(UnityEngine.Vector3), typeof(float), typeof(UnityEngine.Collider[]), typeof(int)])),
            new CodeMatch(OpCodes.Stloc_1))
        .ThrowIfInvalid($"Could not patch EffectArea.GetBaseValue()! (overlap-sphere-non-alloc)")
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call,
                AccessTools.Method(typeof(EffectAreaPatch), nameof(OverlapSphereNonAllocDelegate))))
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(EffectArea.IsPointInsideArea))]
  static IEnumerable<CodeInstruction> IsPointInsideAreaTranspiler(IEnumerable<CodeInstruction> instructions) {
    if (!IsModEnabled.Value) {
      return instructions;
    }

    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldsfld),
            new CodeMatch(OpCodes.Call, AccessTools.Method(
                typeof(UnityEngine.Physics), 
                nameof(UnityEngine.Physics.OverlapSphereNonAlloc), 
                [typeof(UnityEngine.Vector3), typeof(float), typeof(UnityEngine.Collider[]), typeof(int)])),
            new CodeMatch(OpCodes.Stloc_0))
        .ThrowIfInvalid($"Could not patch EffectArea.IsPointInsideArea()! (overlap-sphere-non-alloc)")
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call,
                AccessTools.Method(typeof(EffectAreaPatch), nameof(OverlapSphereNonAllocDelegate))))
        .InstructionEnumeration();
  }

  static int OverlapSphereNonAllocDelegate(int count) {
    if (count == EffectArea.m_tempColliders.Length) {
      Effectual.LogInfo($"EffectArea.m_tempColliders buffer full at size {count}, increasing by 128.");
      Array.Resize(ref EffectArea.m_tempColliders, count + 128);
    }

    return count;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(EffectArea.CustomFixedUpdate))]
  static IEnumerable<CodeInstruction> CustomFixedUpdateTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    if (!IsModEnabled.Value) {
      return instructions;
    }

    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(EffectArea), nameof(EffectArea.m_statusEffectHash))))
        .ThrowIfInvalid($"Could not patch EffectArea.CustomFixedUpdate()! (add-status-effect)")
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_1))
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call,
                AccessTools.Method(typeof(EffectAreaPatch), nameof(AddStatusEffectDelegate))))
        .InstructionEnumeration();
  }

  static bool AddStatusEffectDelegate(Character character, int statusEffectHash) {
    return statusEffectHash != 0 && character && character.m_nview.IsValid();
  }
}
