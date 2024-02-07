namespace BetterZeeLog;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

[HarmonyPatch(typeof(ZLog))]
static class ZLogPatch {
  static string DateTimeNowDelegate(string dateTimeNow) {
    return "[" + dateTimeNow + "] ";
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZLog.Log))]
  static bool LogPrefix(ref object o) {
    if (o.ToString().StartsWith("Console: ")) {
      return false;
    }

    return true;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZLog.Log))]
  static IEnumerable<CodeInstruction> LogTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchForward(
            useEnd: true,
            new CodeMatch(OpCodes.Ldstr, ": "))
        .ThrowIfInvalid("Could not patch ZLog.Log() colon instruction.")
        .InsertAndAdvance(Transpilers.EmitDelegate(DateTimeNowDelegate))
        .SetOperandAndAdvance(string.Empty)
        .MatchForward(
            useEnd: true,
            new CodeMatch(OpCodes.Ldstr, "\n"))
        .ThrowIfInvalid("Could not patch ZLog.Log() newline instruction.")
        .SetOperandAndAdvance(string.Empty)
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZLog.LogWarning))]
  static IEnumerable<CodeInstruction> LogWarningTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchForward(
            useEnd: true,
            new CodeMatch(OpCodes.Ldstr, ": "))
        .ThrowIfInvalid("Could not patch ZLog.LogWarning() colon instruction.")
        .InsertAndAdvance(Transpilers.EmitDelegate(DateTimeNowDelegate))
        .SetOperandAndAdvance(string.Empty)
        .MatchForward(
            useEnd: true,
            new CodeMatch(OpCodes.Ldstr, "\n"))
        .ThrowIfInvalid("Could not patch ZLog.LogWarning() newline instruction.")
        .SetOperandAndAdvance(string.Empty)
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZLog.LogError))]
  static IEnumerable<CodeInstruction> LogErrorTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchForward(
            useEnd: true,
            new CodeMatch(OpCodes.Ldstr, ": "))
        .ThrowIfInvalid("Could not patch ZLog.LogError() colon instruction.")
        .InsertAndAdvance(Transpilers.EmitDelegate(DateTimeNowDelegate))
        .SetOperandAndAdvance(string.Empty)
        .MatchForward(
            useEnd: true,
            new CodeMatch(OpCodes.Ldstr, "\n"))
        .ThrowIfInvalid("Could not patch ZLog.LogError() newline instruction.")
        .SetOperandAndAdvance(string.Empty)
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZLog.DevLog))]
  static IEnumerable<CodeInstruction> DevLogTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchForward(
            useEnd: true,
            new CodeMatch(OpCodes.Ldstr, ": "))
        .ThrowIfInvalid("Could not patch ZLog.DevLog() colon instruction.")
        .InsertAndAdvance(Transpilers.EmitDelegate(DateTimeNowDelegate))
        .SetOperandAndAdvance(string.Empty)
        .MatchForward(
            useEnd: true,
            new CodeMatch(OpCodes.Ldstr, "\n"))
        .ThrowIfInvalid("Could not patch ZLog.DevLog() newline instruction.")
        .SetOperandAndAdvance(string.Empty)
        .InstructionEnumeration();
  }
}
