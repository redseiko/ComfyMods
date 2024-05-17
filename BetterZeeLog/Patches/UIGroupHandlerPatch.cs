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
        .ThrowIfInvalid("Could not patch UIGroupHandler.Update()! (Log-Right-Stick)")
        .Advance(offset: 1)
        .CreateLabel(out Label activateRightStickLabel)
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZLog), nameof(ZLog.Log))))
        .ThrowIfInvalid("Could not patch UIGroupHandler.Update()! (Log-Default)")
        .Advance(offset: 1)
        .CreateLabel(out Label activeDefaultLabel)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "Activating right stick element "))
        .ThrowIfInvalid($"Could not patch UIGroupHandler.Update()! (Ldstr-Right-Stick)")
        .InsertAndAdvance(
            Transpilers.EmitDelegate(ShouldLogDelegate),
            new CodeInstruction(OpCodes.Brfalse, activateRightStickLabel))
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "Activating default element "))
        .ThrowIfInvalid($"Could not patch UIGroupHandler.Update()! (Ldstr-Default)")
        .InsertAndAdvance(
            Transpilers.EmitDelegate(ShouldLogDelegate),
            new CodeInstruction(OpCodes.Brfalse, activeDefaultLabel))
        .InstructionEnumeration();
  }

  static bool ShouldLogDelegate() {
    return !IsModEnabled.Value;
  }
}
