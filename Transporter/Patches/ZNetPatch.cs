using HarmonyLib;

namespace Transporter {
  [HarmonyPatch(typeof(ZNet))]
  static class ZNetPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZNet.UpdatePlayerList))]
    static void UpdatePlayerListPostfix(ZNet __instance) {
      PlayerUtils.RefreshPlayerIdMapping();
      TeleportManager.ProcessPendingTeleports();
    }
  }
}
