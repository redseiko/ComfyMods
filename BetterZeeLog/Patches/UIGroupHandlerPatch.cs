namespace BetterZeeLog;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

[HarmonyPatch(typeof(UIGroupHandler))]
static class UIGroupHandlerPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(UIGroupHandler.Update))]
  static IEnumerable<CodeInstruction> Update1Transpiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZLog), nameof(ZLog.Log))))
        .ThrowIfNotMatch("Could not patch UIGroupHandler.Update()! (log-right-stick)")
        .SetInstruction(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(UIGroupHandlerPatch), nameof(LogDelegate))))
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZLog), nameof(ZLog.Log))))
        .ThrowIfNotMatch("Could not patch UIGroupHandler.Update()! (log-default)")
        .SetInstruction(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(UIGroupHandlerPatch), nameof(LogDelegate))))
        .InstructionEnumeration();
  }

  static void LogDelegate(object obj) {
    // ...
  }
}
