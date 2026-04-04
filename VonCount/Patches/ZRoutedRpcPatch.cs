namespace VonCount;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

[HarmonyPatch(typeof(ZRoutedRpc))]
static class ZRoutedRpcPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZRoutedRpc.RPC_RoutedRPC))]
  static IEnumerable<CodeInstruction> RPCRoutedRPCTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(
                OpCodes.Callvirt,
                AccessTools.Method(typeof(ZRoutedRpc.RoutedRPCData), nameof(ZRoutedRpc.RoutedRPCData.Deserialize))))
        .ThrowIfInvalid($"Could not patch (ZRoutedRpc.RPC_RoutedRPC()! (rpc-data-deserialize)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(CountManager), nameof(CountManager.CountRoutedRPC))))
        .InstructionEnumeration();
  }
}
