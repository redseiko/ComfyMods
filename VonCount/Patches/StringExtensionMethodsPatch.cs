namespace VonCount;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

[HarmonyPatch(typeof(StringExtensionMethods))]
static class StringExtensionMethodsPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(StringExtensionMethods.GetStableHashCode))]
  static IEnumerable<CodeInstruction> GetStableHashCodeTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .End()
        .MatchStartBackwards(OpCodes.Ret)
        .ThrowIfInvalid($"Could not patch StringExtensionMethods.GetStableHashCode()! (dup-ret)")
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(CountManager), nameof(CountManager.CacheStableHashCode))))
        .InstructionEnumeration();
  }
}
