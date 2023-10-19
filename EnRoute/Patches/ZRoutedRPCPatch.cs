using HarmonyLib;

namespace EnRoute {
  [HarmonyPatch(typeof(ZRoutedRpc))]
  static class ZRoutedRPCPatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ZRoutedRpc.AddPeer))]
    static void AddPeerPrefix(ZNetPeer peer) {
      RouteManager.OnAddPeer(peer);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZRoutedRpc.RemovePeer))]
    static void RemovePeerPostfix(ZNetPeer peer) {
      RouteManager.OnRemovePeer(peer);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(ZRoutedRpc.RouteRPC))]
    static bool RouteRPCPrefix(ZRoutedRpc __instance, ZRoutedRpc.RoutedRPCData rpcData) {
      if (__instance.m_server) {
        RouteManager.RouteRPC(__instance, rpcData);
        return false;
      }

      return true;
    }
  }
}
