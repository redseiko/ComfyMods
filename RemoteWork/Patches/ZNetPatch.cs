namespace RemoteWork;

using HarmonyLib;

[HarmonyPatch(typeof(ZNet))]
static class ZNetPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZNet.RPC_RemoteCommand))]
  static bool RPC_RemoteCommandPrefix(ZNet __instance, ZRpc rpc, string command) {
    string steamId = rpc.m_socket.GetHostName();

    if (AccessUtils.HasAccess(steamId)) {
      AccessLogger.Log("OK", steamId, command);
      CommandUtils.RunCommand(rpc, command);
    } else {
      AccessLogger.Log("NOAUTH", steamId, command);
    }

    return false;
  }
}
