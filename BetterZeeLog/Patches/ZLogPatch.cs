namespace BetterZeeLog;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

[HarmonyPatch(typeof(ZLog))]
static class ZLogPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZLog.Log))]
  static IEnumerable<CodeInstruction> LogTranspiler(IEnumerable<CodeInstruction> instructions) {
    return LogFormatTranspiler(instructions, nameof(ZLog.Log));
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZLog.LogWarning))]
  static IEnumerable<CodeInstruction> LogWarningTranspiler(IEnumerable<CodeInstruction> instructions) {
    return LogFormatTranspiler(instructions, nameof(ZLog.LogWarning));
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZLog.LogError))]
  static IEnumerable<CodeInstruction> LogErrorTranspiler(IEnumerable<CodeInstruction> instructions) {
    return LogFormatTranspiler(instructions, nameof(ZLog.LogError));
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZLog.DevLog))]
  static IEnumerable<CodeInstruction> DevLogTranspiler(IEnumerable<CodeInstruction> instructions) {
    return LogFormatTranspiler(instructions, nameof(ZLog.DevLog));
  }

  static IEnumerable<CodeInstruction> LogFormatTranspiler(
      IEnumerable<CodeInstruction> instructions, string methodName) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(
                OpCodes.Call,
                AccessTools.PropertyGetter(typeof(CultureInfo), nameof(CultureInfo.InvariantCulture))),
            new CodeMatch(
                OpCodes.Call,
                AccessTools.Method(typeof(DateTime), nameof(DateTime.ToString), [typeof(IFormatProvider)])),
            new CodeMatch(OpCodes.Ldstr, ": "))
        .ThrowIfNotMatch($"Could not patch ZLog.{methodName}()! (format)")
        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldstr, "[MM/dd/yyyy HH:mm:ss] "))
        .Advance(offset: 1)
        .SetOperandAndAdvance(
            AccessTools.Method(typeof(DateTime), nameof(DateTime.ToString), [typeof(string), typeof(IFormatProvider)]))
        .RemoveInstruction()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "\n"),
            new CodeMatch(
                OpCodes.Call,
                AccessTools.Method(
                    typeof(string),
                    nameof(string.Concat),
                    [typeof(string), typeof(string), typeof(string), typeof(string)])))
        .ThrowIfNotMatch($"Could not patch ZLog.{methodName}()! (newline)")
        .ExtractLabels(out List<Label> newlineLabels)
        .RemoveInstruction()
        .AddLabels(newlineLabels)
        .SetOperandAndAdvance(
            AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)]))
        .InstructionEnumeration();
  }
}
