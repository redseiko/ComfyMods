using System;

using HarmonyLib;

namespace EnRoute {
  [HarmonyPatch(typeof(ZNet))]
  static class ZNetPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZNet.UpdateNetTime))]
    static void UpdateNetTime(ZNet __instance) {
      EnRoute.NetTimeTicks = (long) __instance.m_netTime * TimeSpan.TicksPerSecond;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZNet.UpdatePlayerList))]
    static void UpdatePlayerListPostfix() {
      RouteManager.RefreshRouteRecords();
    }
  }
}
