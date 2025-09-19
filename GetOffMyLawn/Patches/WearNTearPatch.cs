namespace GetOffMyLawn;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(WearNTear))]
static class WearNTearPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(WearNTear.ApplyDamage))]
  static IEnumerable<CodeInstruction> ApplyDamageTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Ldc_R4, 0f),
            new CodeMatch(OpCodes.Bgt_Un),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Ret))
        .ThrowIfInvalid($"Could not patch WearNTear.ApplyDamage()! (health-lt-0)")
        .Advance(offset: 5)
        .ExtractLabels(out List<Label> postHealthLt0Labels)
        .Insert(
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(WearNTearPatch), nameof(IsHealthGreaterThanDamageThreshold))),
            new CodeInstruction(OpCodes.Brfalse),
            new CodeInstruction(OpCodes.Ldc_I4_0),
            new CodeInstruction(OpCodes.Ret))
        .AddLabels(postHealthLt0Labels)
        .CreateLabelOffset(offset: 5, out Label postHealthGtDamageLabel)
        .Advance(offset: 2)
        .SetOperandAndAdvance(postHealthGtDamageLabel)
        .InstructionEnumeration();
  }

  public const float PieceHealthDamageThreshold = 100_000f;

  static bool IsHealthGreaterThanDamageThreshold(float health) {
    if (health >= PieceHealthDamageThreshold && IsModEnabled.Value && EnablePieceHealthDamageThreshold.Value) {
      return true;
    }

    return false;
  }
}
