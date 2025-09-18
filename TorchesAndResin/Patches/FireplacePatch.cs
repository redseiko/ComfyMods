namespace TorchesAndResin;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Fireplace))]
static class FireplacePatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Fireplace.Awake))]
  static IEnumerable<CodeInstruction> AwakeTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Fireplace), nameof(Fireplace.m_nview))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ZNetView), nameof(ZNetView.IsOwner))))
        .ThrowIfInvalid($"Could not patch Fireplace.Awake()! (check-is-owner")
        .ExtractLabels(out List<Label> checkIsOwnerLabels)
        .Insert(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(FireplacePatch), nameof(CheckIsOwnerPreDelegate))))
        .AddLabels(checkIsOwnerLabels)
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "UpdateFireplace"),
            new CodeMatch(OpCodes.Ldc_R4, 0f),
            new CodeMatch(OpCodes.Ldc_R4, 2f))
        .ThrowIfInvalid($"Could not patch Fireplace.Awake()! (invoke-update-fireplace)")
        .Advance(offset: 1)
        .SetOperandAndAdvance(0.5f)
        .InstructionEnumeration();
  }

  static void CheckIsOwnerPreDelegate(Fireplace fireplace) {
    if (IsModEnabled.Value) {
      FireplaceManager.SetEligibleFireplaceFuel(fireplace, TorchStartingFuel.Value);
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Fireplace.GetHoverText))]
  static IEnumerable<CodeInstruction> GetHoverTextTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_1),
            new CodeMatch(OpCodes.Ldstr, "\n[<color=yellow><b>$KEY_Use</b></color>] $piece_use"),
            new CodeMatch(OpCodes.Call),
            new CodeMatch(OpCodes.Stloc_1))
        .ThrowIfInvalid($"Could not patch Fireplace.GetHoverText()! (can-turn-off-text)")
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(FireplacePatch), nameof(CanTurnOffHoverTextDelegate))))
        .InstructionEnumeration();
  }

  static string CanTurnOffHoverTextDelegate(string hoverText, float fuel) {
    if (IsModEnabled.Value && CandleHoverTextShowFuel.Value) {
      return $"\n( $piece_fire_fuel {fuel:F2} )\n[<color=yellow><b>$KEY_Use</b></color>] $piece_use";
    }

    return hoverText;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Fireplace.UpdateState))]
  static void UpdateStatePrefix(Fireplace __instance, ref bool __state) {
    if (__instance.m_canTurnOff
        && __instance.m_wet
        && IsModEnabled.Value
        && CandleUpdateStateNeverWet.Value
        && __instance.m_startFuel > __instance.m_maxFuel) {
      __state = true;
      __instance.m_wet = false;
    } else {
      __state = false;
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Fireplace.UpdateState))]
  static void UpdateStatePostfix(Fireplace __instance, bool __state) {
    if (__state) {
      __instance.m_wet = true;
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Fireplace.IsBurning))]
  static IEnumerable<CodeInstruction> IsBuringTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Floating), nameof(Floating.IsUnderWater))))
        .ThrowIfInvalid($"Could not patch Fireplace.IsBurning()! (is-under-water-check)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(FireplacePatch), nameof(IsUnderWaterPostDelegate))))
        .InstructionEnumeration();
  }

  static bool IsUnderWaterPostDelegate(bool isUnderWater, Fireplace fireplace) {
    if (isUnderWater
        && fireplace.m_canTurnOff
        && IsModEnabled.Value
        && CandleUpdateStateNeverWet.Value
        && fireplace.m_startFuel > fireplace.m_maxFuel) {
      return false;
    }

    return isUnderWater;
  }
}
