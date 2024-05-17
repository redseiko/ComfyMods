namespace Parrot;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

[HarmonyPatch(typeof(ZNet))]
static class ZNetPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZNet.CheckForIncommingServerConnections))]
  static IEnumerable<CodeInstruction> CheckForIncommingServerConnectionsTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldloc_1),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZNet), nameof(ZNet.OnNewConnection))))
        .ThrowIfInvalid("Could not patch ZNet.CheckForIncommingServerConnections()! (OnNewConnection)")
        .CreateLabel(out Label onNewConnectionLabel)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_1),
            Transpilers.EmitDelegate(OnNewConnectionDelegate),
            new CodeInstruction(OpCodes.Brfalse, onNewConnectionLabel),
            new CodeInstruction(OpCodes.Ret))
        .InstructionEnumeration();
  }

  static bool OnNewConnectionDelegate(ZNetPeer netPeer) {
    if (ConnectionManager.IsSteamGameServer(netPeer)) {
      ConnectionManager.RegisterParrotClient(netPeer);
      return true;
    }

    return false;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZNet.Disconnect))]
  static void DisconnectPrefix(ZNetPeer peer) {
    ConnectionManager.ParrotNetPeers.Remove(peer);
  }
}
