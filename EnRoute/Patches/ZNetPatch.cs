using System.Collections;
using System.Diagnostics;

using HarmonyLib;

using UnityEngine;

namespace EnRoute {
  [HarmonyPatch(typeof(ZNet))]
  static class ZNetPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZNet.RPC_PlayerList))]
    static void RPC_PlayerListPostfix() {
      RouteNearbyManager.RefreshNearbyPlayers();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZNet.UpdatePlayerList))]
    static void UpdatePlayerListPostfix() {
      RouteManager.RefreshRouteRecords();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZNet.Awake))]
    static void AwakePostfix(ZNet __instance) {
      if (ZNet.m_isServer) {
        // Server logging code goes here.
      } else {
        __instance.StartCoroutine(LogStatsCoroutine());
      }
    }

    static IEnumerator LogStatsCoroutine() {
      WaitForSeconds waitInterval = new(seconds: 60f);
      Stopwatch stopwatch = Stopwatch.StartNew();

      while (true) {
        yield return waitInterval;
        RouteToStats.LogStats(stopwatch.Elapsed);
      }
    }
  }
}
