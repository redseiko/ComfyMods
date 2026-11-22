namespace Eventuality;

using HarmonyLib;

using UnityEngine;

[HarmonyPatch(typeof(RandEventSystem))]
static class RandEventSystemPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(RandEventSystem.RPC_SetEvent))]
  static bool RPC_SetEventPrefix(
      RandEventSystem __instance, long sender, string eventName, float time, Vector3 pos) {
    if (!ZNet.m_isServer) {
      return true;
    }

    // Ignore `RPC_SetEvent` when triggered by server in `SendCurrentRandomEvent()`.
    if (sender == ZRoutedRpc.s_instance.m_id) {
      return false;
    }

    RandEventManager.ProcessSetEventRequest(__instance, sender, eventName, time, pos);
    return false;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(RandEventSystem.SetRandomEvent))]
  static bool SetRandomEventPrefix(RandEventSystem __instance, RandomEvent ev, Vector3 pos) {
    if (!ZNet.m_isServer) {
      return true;
    }

    RandEventManager.SetRandomEvent(__instance, ev, pos);
    return false;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(RandEventSystem.InValidBiome))]
  static bool InValidBiomePrefix(RandomEvent ev, Vector3 point, ref bool __result) {
    if (!ZNet.m_isServer) {
      return true;
    }

    __result = RandEventManager.IsInValidBiome(ev, point);
    return false;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(RandEventSystem.CheckBase))]
  static bool CheckBasePrefix(RandomEvent ev, RandEventSystem.PlayerEventData player, ref bool __result) {
    if (!ZNet.m_isServer) {
      return true;
    }

    __result = RandEventManager.CheckBase(ev, player);
    return false;
  }
}
