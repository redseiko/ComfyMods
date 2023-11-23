using ComfyLib;

using HarmonyLib;

namespace HoldMyMead {
  [HarmonyPatch(typeof(ZRoutedRpc))]
  static class ZRoutedRpcPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZRoutedRpc.RPC_RoutedRPC))]
    static void RPC_RoutedRPCPostfix(ZRoutedRpc __instance, ZRpc rpc, ZPackage pkg) {
      pkg.SetPos(0);
      ZRoutedRpc.RoutedRPCData rpcData = pkg.ReadRoutedRPCData();

      if (rpcData.m_methodHash == PlayerDeathManager.OnDeathHashCode) {
        PlayerDeathManager.ProcessOnDeathRPC(rpcData);
      }
    }
  }
}
