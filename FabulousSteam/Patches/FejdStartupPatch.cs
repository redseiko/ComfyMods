namespace FabulousSteam;

using HarmonyLib;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.ParseServerArguments))]
  static void ParseServerArgumentsPostfix(bool __result) {
    if (__result) {
      ZPlayFabMatchmaking.LookupPublicIP();
      ZNet.m_onlineBackend = OnlineBackendType.Steamworks;
    }
  }
}
