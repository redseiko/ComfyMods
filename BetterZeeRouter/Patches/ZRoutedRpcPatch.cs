namespace BetterZeeRouter;

using HarmonyLib;

[HarmonyPatch(typeof(ZRoutedRpc))]
static class ZRoutedRpcPatch {
  static readonly ZRoutedRpc.RoutedRPCData _routedRpcData = new();

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZRoutedRpc.RPC_RoutedRPC))]
  static bool RPC_RoutedRPCPrefix(ref ZRoutedRpc __instance, ref ZRpc rpc, ref ZPackage pkg) {
    _routedRpcData.DeserializeFrom(ref pkg);

    if (_routedRpcData.m_targetPeerID == __instance.m_id || _routedRpcData.m_targetPeerID == 0L) {
      __instance.HandleRoutedRPC(_routedRpcData);
    }

    if (!__instance.m_server || _routedRpcData.m_targetPeerID == __instance.m_id) {
      return false;
    }

    if (RoutedRpcManager.Instance.Process(_routedRpcData)) {
      __instance.RouteRPC(_routedRpcData);
    }

    return false;
  }
}
