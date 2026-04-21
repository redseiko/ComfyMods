namespace ConstructionDerby;

using System;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ZNet))]
static class ZNetPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNet.Awake))]
  static void AwakePostfix(ref ZNet __instance) {
    if (!IsModEnabled.Value) {
      return;
    }

    __instance.m_routedRpc.Register("StartDerbyGame", new Action<long, ZPackage>(DerbyManager.RPC_StartDerbyGame));
    __instance.m_routedRpc.Register("StopDerbyGame", new Action<long>(DerbyManager.RPC_StopDerbyGame));
  }
}
