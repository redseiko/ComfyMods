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
}
