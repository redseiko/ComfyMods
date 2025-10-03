namespace Keysential;

using HarmonyLib;

[HarmonyPatch(typeof(ZRoutedRpc))]
static class ZRoutedRpcPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZRoutedRpc.RemovePeer))]
  static void RemovePeerPostfix(ZNetPeer peer) {
    if (ZNet.m_isServer) {
      GlobalKeysManager.RemovePeer(peer);
    }
  }
}
