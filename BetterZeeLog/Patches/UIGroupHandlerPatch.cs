namespace BetterZeeLog;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(UIGroupHandler))]
static class UIGroupHandlerPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(UIGroupHandler.Update))]
  static IEnumerable<CodeInstruction> UpdateTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZLog), nameof(ZLog.Log))))
        .ThrowIfNotMatch("Could not patch UIGroupHandler.Update()! (log-right-stick)")
        .Advance(offset: 1)
        .CreateLabel(out Label activateRightStickLabel)
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZLog), nameof(ZLog.Log))))
        .ThrowIfNotMatch("Could not patch UIGroupHandler.Update()! (log-default)")
        .Advance(offset: 1)
        .CreateLabel(out Label activeDefaultLabel)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "Activating right stick element "))
        .ThrowIfNotMatch($"Could not patch UIGroupHandler.Update()! (ldstr-right-stick)")
        .InsertAndAdvance(
            Transpilers.EmitDelegate(ShouldLogDelegate),
            new CodeInstruction(OpCodes.Brfalse, activateRightStickLabel))
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "Activating default element "))
        .ThrowIfNotMatch($"Could not patch UIGroupHandler.Update()! (ldstr-default)")
        .InsertAndAdvance(
            Transpilers.EmitDelegate(ShouldLogDelegate),
            new CodeInstruction(OpCodes.Brfalse, activeDefaultLabel))
        .InstructionEnumeration();
  }

  static bool ShouldLogDelegate() {
    return !IsModEnabled.Value;
  }
}
