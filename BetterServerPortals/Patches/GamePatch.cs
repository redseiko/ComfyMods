namespace BetterServerPortals;

using HarmonyLib;

[HarmonyPatch(typeof(Game))]
static class GamePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Game.Start))]
  static void StartPostfix(Game __instance) {
    if (ZNet.m_isServer) {
      __instance.StopCoroutine(nameof(Game.ConnectPortalsCoroutine));
      __instance.StartCoroutine(PortalManager.ConnectPortalsCoroutine());
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Game.ConnectPortals))]
  static bool ConnectPortalsPrefix() {
    PortalManager.ConnectPortals(ZDOMan.s_instance);
    return false;
  }
}
