namespace VonCount;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

[HarmonyPatch(typeof(ZNetScene))]
static class ZNetScenePatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZNetScene.Destroy))]
  static IEnumerable<CodeInstruction> DestroyTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ZDOMan), nameof(ZDOMan.DestroyZDO))))
        .ThrowIfInvalid($"Could not patch (ZDOMan.DestroyZDO)! (destroy-zdo)")
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(CountManager), nameof(CountManager.CountDestroyZDO))))
        .InstructionEnumeration();
  }
}
