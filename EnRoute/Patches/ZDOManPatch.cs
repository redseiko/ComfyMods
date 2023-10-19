using System;

using HarmonyLib;

namespace EnRoute {
  [HarmonyPatch(typeof(ZDOMan))]
  static class ZDOManPatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ZDOMan.HandleDestroyedZDO))]
    static bool HandleDestroyedZDOPrefix(ref ZDOMan __instance, ZDOID uid) {
      if (uid == ZDOID.None) {
        return false;
      }

      if (uid.UserID == __instance.m_sessionID && uid.ID >= __instance.m_nextUid) {
        __instance.m_nextUid = uid.ID + 1U;
      }

      if (!__instance.m_objectsByID.TryGetValue(uid, out ZDO zdo) || zdo == null) {
        return false;
      }

      __instance.m_onZDODestroyed(zdo);

      __instance.RemoveFromSector(zdo, zdo.GetSector());
      __instance.m_objectsByID.Remove(zdo.m_uid);

      if (Game.instance.PortalPrefabHash.Contains(zdo.m_prefab)) {
        __instance.m_portalObjects.Remove(zdo);
      }

      ZDOPool.Release(zdo);

      foreach (ZDOMan.ZDOPeer zdoPeer in __instance.m_peers) {
        if (zdoPeer.m_zdos.Remove(uid)) {
          // TODO: need to route this RPC for any matching NetPeers.
        }
      }

      if (ZNet.m_isServer) {
        __instance.m_deadZDOs[uid] = EnRoute.NetTimeTicks;
      }

      return false;
    }
  }
}
