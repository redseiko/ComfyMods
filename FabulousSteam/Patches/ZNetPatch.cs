namespace FabulousSteam;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

[HarmonyPatch(typeof(ZNet))]
static class ZNetPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZNet.OpenServer))]
  static bool OpenServerPrefix(ZNet __instance) {
    if (ZNet.m_isServer) {
      BackendManager.OpenServers(__instance);
    }

    return false;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNet.StopAll))]
  static void StopAllPostfix(ZNet __instance) {
    BackendManager.StopServers(__instance);
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZNet.CheckForIncommingServerConnections))]
  static bool CheckForIncommingServerConnectionsPrefix(ZNet __instance) {
    BackendManager.ProcessIncomingServerConnections(__instance);
    return false;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZNet.RPC_PeerInfo))]
  static void RPC_PeerInfoPrefix(ZRpc rpc) {
    if (rpc.m_socket is ZPlayFabSocket) {
      ZNet.m_onlineBackend = OnlineBackendType.PlayFab;
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNet.RPC_PeerInfo))]
  static void RPC_PeerInfoPostfix() {
    ZNet.m_onlineBackend = OnlineBackendType.Steamworks;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZNet.UpdatePlayerList))]
  static IEnumerable<CodeInstruction> UpdatePlayerListTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldsfld, AccessTools.Field(typeof(ZNet), nameof(ZNet.m_onlineBackend))))
        .ThrowIfInvalid($"Could not patch ZNet.UpdatePlayerList()! (compare-online-backend)")
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_2))
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(ZNetPatch), nameof(CompareOnlineBackendDelegate))))
        .InstructionEnumeration();
  }

  static OnlineBackendType CompareOnlineBackendDelegate(ZNetPeer netPeer, OnlineBackendType onlineBackend) {
    if (netPeer.m_socket is ZPlayFabSocket) {
      return OnlineBackendType.PlayFab;
    }

    return onlineBackend;
  }
}
