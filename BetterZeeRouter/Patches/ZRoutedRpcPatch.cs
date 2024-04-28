namespace BetterZeeRouter;

using HarmonyLib;

[HarmonyPatch(typeof(ZRoutedRpc))]
static class ZRoutedRpcPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZRoutedRpc.RPC_RoutedRPC))]
  static bool RPC_RoutedRPCPrefix(ZRoutedRpc __instance, ZRpc rpc, ZPackage pkg) {
    if (__instance.m_server) {
      RoutedRpcManager.ProcessRoutedRPC(__instance, rpc, pkg);
      return false;
    }

    return true;
  }
}
