namespace Keysential;

using HarmonyLib;

[HarmonyPatch(typeof(ZoneSystem))]
static class ZoneSystemPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZoneSystem.Load))]
  static void LoadPostfix(ZoneSystem __instance) {
    if (ZNet.m_isServer) {
      ZoneSystemManager.SetupGlobalKeys(__instance);
      KeyManagerUtils.RunStartUpKeyManagers();
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZoneSystem.RPC_SetGlobalKey))]
  static bool RPC_SetGlobalKeyPrefix(ZoneSystem __instance, long sender, string name) {
    if (ZNet.m_isServer && ZoneSystemManager.ShouldIgnoreSetGlobalKey(__instance, sender, name)) {
      return false;
    }

    return true;
  }
}
