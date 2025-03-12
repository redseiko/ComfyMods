namespace BetterZeeRouter;

using HarmonyLib;

[HarmonyPatch(typeof(ZNet))]
static class ZNetPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNet.Start))]
  static void StartPostfix() {
    RoutedRpcManager.SetupServerPeer();
  }
}
