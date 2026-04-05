namespace VonCount;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ZNet))]
static class ZNetPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNet.RPC_PlayerList))]
  static void RPC_PlayerListPostfix(ZNet __instance) {
    if (IsModEnabled.Value) {
      CountManager.ProcessPlayerListUpdate(__instance.m_players);
    }
  }
}
