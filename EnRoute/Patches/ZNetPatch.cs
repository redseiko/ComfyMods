namespace EnRoute;

using System;

using HarmonyLib;

[HarmonyPatch(typeof(ZNet))]
static class ZNetPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNet.UpdateNetTime))]
  static void UpdateNetTime(ZNet __instance) {
    EnRouteManager.NetTimeTicks = (long) __instance.m_netTime * TimeSpan.TicksPerSecond;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNet.UpdatePlayerList))]
  static void UpdatePlayerListPostfix() {
    RouteManager.RefreshRouteRecords();
  }
}
