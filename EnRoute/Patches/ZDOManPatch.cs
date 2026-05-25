namespace EnRoute;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

[HarmonyPatch(typeof(ZDOMan))]
static class ZDOManPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZDOMan.HandleDestroyedZDO))]
  static IEnumerable<CodeInstruction> HandleDestroyedZDOTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(ZNet), nameof(ZNet.instance))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ZNet), nameof(ZNet.GetTime))),
            new CodeMatch(OpCodes.Stloc_S),
            new CodeMatch(OpCodes.Ldloca_S),
            new CodeMatch(
                OpCodes.Call, AccessTools.PropertyGetter(typeof(System.DateTime), nameof(System.DateTime.Ticks))),
            new CodeMatch(OpCodes.Stloc_S))
        .ThrowIfInvalid($"Could not patch ZDOMan.HandleDestroyedZDO()! (get-time-ticks)")
        .SetInstructionAndAdvance(
            new CodeInstruction(
                OpCodes.Ldsfld, AccessTools.Field(typeof(EnRouteManager), nameof(EnRouteManager.NetTimeTicks))))
        .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
        .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
        .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
        .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
        .InstructionEnumeration();
  }
}
