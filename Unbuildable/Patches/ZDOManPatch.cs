namespace Unbuildable;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

[HarmonyPatch(typeof(ZDOMan))]
static class ZDOManPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZDOMan.RPC_ZDOData))]
  static IEnumerable<CodeInstruction> RPC_ZDODataTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldloc_3),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ZDO), nameof(ZDO.Deserialize))))
        .ThrowIfInvalid("Could not patch ZDOMan.RPC_ZDOData()! (Deserialize)")
        .SaveInstruction(offset: 0, out CodeInstruction ldLocS12)
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ZDOMan), nameof(ZDOMan.m_deadZDOs))),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(
                OpCodes.Callvirt,
                AccessTools.Method(typeof(Dictionary<ZDOID, long>), nameof(Dictionary<ZDOID, long>.ContainsKey))))
        .ThrowIfInvalid("Could not patch ZDOMan.RPC_ZDOData()! (ContainsKey)")
        .Advance(offset: 3)
        .InsertAndAdvance(
            ldLocS12,
            Transpilers.EmitDelegate(ZDOManUtils.DestroyZDOsDelegate))
        .InstructionEnumeration();
  }
}
