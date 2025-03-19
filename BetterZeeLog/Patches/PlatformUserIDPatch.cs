namespace BetterZeeLog;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using Splatform;

[HarmonyPatch(typeof(PlatformUserID))]
static class PlatformUserIDPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(MethodType.Constructor, argumentTypes: [typeof(string)])]
  static IEnumerable<CodeInstruction> ConstructorTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "PlatformUserID \""),
            new CodeMatch(OpCodes.Ldarg_1),
            new CodeMatch(OpCodes.Ldstr, "\" failed to parse!"))
        .ThrowIfNotMatch($"Could not patch PlatformUserID.Constructor()! (log-failed-to-parse)")
        .CreateLabelOffset(offset: 5, out Label logNoSeparatorLabel)
        .InsertAndAdvance(new CodeInstruction(OpCodes.Br, logNoSeparatorLabel))
        .InstructionEnumeration();
  }
}
