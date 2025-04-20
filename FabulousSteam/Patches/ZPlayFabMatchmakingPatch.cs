namespace FabulousSteam;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ZPlayFabMatchmaking))]
static class ZPlayFabMatchmakingPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZPlayFabMatchmaking.SetDataPort))]
  static void SetDataPortPrefix(ref int serverPort) {
    int playFabServerPort = PlayFabServerPort.Value;

    if (playFabServerPort >= 0) {
      FabulousSteam.LogInfo($"Overriding PlayFab server port from {serverPort} to {playFabServerPort}.");
      serverPort = playFabServerPort;
    }
  }
}
