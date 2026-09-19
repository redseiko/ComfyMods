namespace BetterZeeLog;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

[HarmonyPatch(typeof(ZNetScene))]
static class ZNetScenePatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZNetScene.CreateObject))]
  static IEnumerable<CodeInstruction> CreateObjectTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "Missing prefab hash: "),
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(StringUtils), nameof(StringUtils.StringFromHash))),
            new CodeMatch(
                OpCodes.Call,
                AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZLog), nameof(ZLog.LogWarning))))
        .ThrowIfNotMatch("Could not patch ZNetScene.CreateObject()! (missing-prefab-hash)")
        .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
        .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
        .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
        .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
        .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
        .InstructionEnumeration();
  }
}
