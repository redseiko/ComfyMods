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
            new CodeMatch(OpCodes.Ldstr, "\" failed to parse because it didn't contain a separator!"))
        .ThrowIfNotMatch($"Could not patch PlatformUserID.Constructor()! (log-no-separator)")
        .CreateLabelOffset(offset: 5, out Label logNoSeparatorLabel)
        .InsertAndAdvance(new CodeInstruction(OpCodes.Br, logNoSeparatorLabel))
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "PlatformUserID \""),
            new CodeMatch(OpCodes.Ldarg_1),
            new CodeMatch(OpCodes.Ldstr, "\" failed to parse because it had no platform before the separator!"))
        .ThrowIfNotMatch($"Could not patch PlatformUserID.Constructor()! (log-no-platform)")
        .CreateLabelOffset(offset: 5, out Label logNoPlatformLabel)
        .InsertAndAdvance(new CodeInstruction(OpCodes.Br, logNoPlatformLabel))
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "PlatformUserID \""),
            new CodeMatch(OpCodes.Ldarg_1),
            new CodeMatch(OpCodes.Ldstr, "\" failed to parse because it had no ID after the separator!"))
        .ThrowIfNotMatch($"Could not patch PlatformUserID.Constructor()! (log-no-id)")
        .CreateLabelOffset(offset: 5, out Label logNoIdLabel)
        .InsertAndAdvance(new CodeInstruction(OpCodes.Br, logNoIdLabel))
        .InstructionEnumeration();
  }
}
