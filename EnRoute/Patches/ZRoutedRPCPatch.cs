using System.Collections.Generic;

using HarmonyLib;

namespace EnRoute {
  [HarmonyPatch(typeof(ZRoutedRpc))]
  static class ZRoutedRPCPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZRoutedRpc.AddPeer))]
    static void AddPeerPostfix(ZNetPeer peer) {
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
      } else {
        RouteNearbyManager.RouteRPC(__instance, rpcData);
      }

      return false;
    }
  }
}
