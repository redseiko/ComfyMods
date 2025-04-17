namespace FabulousSteam;

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
}
