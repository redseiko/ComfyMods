namespace BetterServerPortals;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using static PluginConfig;

public static class PortalManager {
  static readonly HashSet<ZDOID> _zdosToForceSend = [];
  static readonly Dictionary<string, ZDO> _zdoByTagCache = [];

  public static void ConnectPortals(ZDOMan zdoManager) {
    ClearCaches();

    UpdateUnconnectedPortals(zdoManager);
    UpdateConnectedPortals(zdoManager);
    ForceSendUpdatedPortals(zdoManager);

    ClearCaches();
  }

  static void ClearCaches() {
    _zdosToForceSend.Clear();
    _zdoByTagCache.Clear();
  }

  static void UpdateUnconnectedPortals(ZDOMan zdoManager) {
    long sessionId = zdoManager.m_sessionID;

    foreach (ZDO zdo in zdoManager.m_portalObjects) {
      string portalTag = zdo.GetString(ZDOVars.s_tag, string.Empty);
      ZDOID targetZDOID = zdo.GetConnectionZDOID(ZDOExtraData.ConnectionType.Portal);

      if (targetZDOID.IsNone()) {
        if (portalTag.Length > 0) {
          _zdoByTagCache[portalTag] = zdo;
        }

        continue;
      }

      if (portalTag.Length > 0
          && zdoManager.m_objectsByID.TryGetValue(targetZDOID, out ZDO targetZDO)
          && targetZDO.GetString(ZDOVars.s_tag, string.Empty) == portalTag) {
        continue;
      }

      zdo.SetOwner(sessionId);
      zdo.UpdateConnection(ZDOExtraData.ConnectionType.Portal, ZDOID.None);

      _zdosToForceSend.Add(zdo.m_uid);

      if (portalTag.Length > 0) {
        _zdoByTagCache[portalTag] = zdo;
      }
    }
  }

  static void UpdateConnectedPortals(ZDOMan zdoManager) {
    long sessionId = zdoManager.m_sessionID;

    foreach (ZDO zdo in zdoManager.m_portalObjects) {
      string portalTag = zdo.GetString(ZDOVars.s_tag, string.Empty);

      if (portalTag.Length <= 0
          || zdo.GetConnectionZDOID(ZDOExtraData.ConnectionType.Portal) != ZDOID.None
          || !_zdoByTagCache.TryGetValue(portalTag, out ZDO matchingZDO)
          || matchingZDO == zdo
          || matchingZDO.GetConnectionZDOID(ZDOExtraData.ConnectionType.Portal) != ZDOID.None) {
        continue;
      }

      zdo.SetOwner(sessionId);
      zdo.SetConnection(ZDOExtraData.ConnectionType.Portal, matchingZDO.m_uid);

      matchingZDO.SetOwner(sessionId);
      matchingZDO.SetConnection(ZDOExtraData.ConnectionType.Portal, zdo.m_uid);

      _zdosToForceSend.Add(zdo.m_uid);
      _zdosToForceSend.Add(matchingZDO.m_uid);

      BetterServerPortals.LogInfo($"Connected portals: {zdo.m_uid} <-> {matchingZDO.m_uid}");
    }
  }

  static void ForceSendUpdatedPortals(ZDOMan zdoManager) {
    if (_zdosToForceSend.Count > 0) {
      foreach (ZDOMan.ZDOPeer zdoPeer in zdoManager.m_peers) {
        zdoPeer.m_forceSend.UnionWith(_zdosToForceSend);
      }
    }
  }

  public static IEnumerator ConnectPortalsCoroutine() {
    BetterServerPortals.LogInfo($"Starting ConnectPortals coroutine with cache...");

    WaitForSeconds waitInterval = new(seconds: ConnectPortalCoroutineWait.Value);
    Stopwatch stopwatch = Stopwatch.StartNew();

    while (true) {
      ConnectPortals(ZDOMan.s_instance);

      if (stopwatch.ElapsedMilliseconds >= 60000L) {
        BetterServerPortals.LogInfo($"Processed {ZDOMan.s_instance.m_portalObjects.Count} portals.");
        stopwatch.Restart();
      }

      yield return waitInterval;
    }
  }
}
