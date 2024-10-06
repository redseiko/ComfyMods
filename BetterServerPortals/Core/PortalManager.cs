namespace BetterServerPortals;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using static PluginConfig;

public static class PortalManager {
  public static readonly char RandomTagPrefix = '?';

  static readonly HashSet<ZDOID> _zdosToForceSend = [];
  static readonly Dictionary<string, ZDO> _portalsByTagCache = [];
  static readonly PooledListDictionary<string, ZDO> _randomPortalsByTagCache = [];

  public static void ConnectPortals(ZDOMan zdoManager) {
    ClearCaches();

    UpdateUnconnectedPortals(zdoManager);
    UpdateConnectedPortals(zdoManager);
    UpdateRandomPortals(zdoManager);
    ForceSendUpdatedPortals(zdoManager);

    ClearCaches();
  }

  static void ClearCaches() {
    _zdosToForceSend.Clear();
    _portalsByTagCache.Clear();
    _randomPortalsByTagCache.Clear();
  }

  static bool IsRandomTag(string tag) {
    return tag.Length > 0 && tag[0] == RandomTagPrefix;
  }

  static void UpdateUnconnectedPortals(ZDOMan zdoManager) {
    long sessionId = zdoManager.m_sessionID;

    foreach (ZDO zdo in zdoManager.m_portalObjects) {
      string portalTag = zdo.GetString(ZDOVars.s_tag, string.Empty);

      if (IsRandomTag(portalTag)) {
        _randomPortalsByTagCache.Add(portalTag, zdo);
        continue;
      }

      ZDOID targetZDOID = zdo.GetConnectionZDOID(ZDOExtraData.ConnectionType.Portal);

      if (targetZDOID.IsNone()) {
        if (portalTag.Length > 0) {
          _portalsByTagCache[portalTag] = zdo;
        }

        continue;
      }

      if (portalTag.Length > 0
          && zdoManager.m_objectsByID.TryGetValue(targetZDOID, out ZDO targetZDO)
          && targetZDO.GetString(ZDOVars.s_tag, string.Empty) == portalTag) {
        continue;
      }

      DisconnectPortal(zdo, sessionId);

      if (portalTag.Length > 0) {
        _portalsByTagCache[portalTag] = zdo;
      }
    }
  }

  static void UpdateConnectedPortals(ZDOMan zdoManager) {
    long sessionId = zdoManager.m_sessionID;

    foreach (ZDO zdo in zdoManager.m_portalObjects) {
      string portalTag = zdo.GetString(ZDOVars.s_tag, string.Empty);

      if (IsRandomTag(portalTag)) {
        continue;
      }

      if (portalTag.Length <= 0
          || zdo.GetConnectionZDOID(ZDOExtraData.ConnectionType.Portal) != ZDOID.None
          || !_portalsByTagCache.TryGetValue(portalTag, out ZDO matchingZDO)
          || matchingZDO == zdo
          || matchingZDO.GetConnectionZDOID(ZDOExtraData.ConnectionType.Portal) != ZDOID.None) {
        continue;
      }

      ConnectPortals(zdo, matchingZDO, sessionId);
    }
  }

  static void UpdateRandomPortals(ZDOMan zdoManager) {
    long sessionId = zdoManager.m_sessionID;

    foreach (KeyValuePair<string, List<ZDO>> pair in _randomPortalsByTagCache) {
      string portalTag = pair.Key;
      List<ZDO> portalZDOs = pair.Value;
      int count = portalZDOs.Count;

      if (count <= 0) {
        continue;
      } else if (count == 1) {
        if (portalZDOs[0].GetConnectionZDOID(ZDOExtraData.ConnectionType.Portal) != ZDOID.None) {
          DisconnectPortal(portalZDOs[0], sessionId);
        }
      } else if (count == 2) {
        if (portalZDOs[0].GetConnectionZDOID(ZDOExtraData.ConnectionType.Portal) != portalZDOs[1].m_uid
            || portalZDOs[1].GetConnectionZDOID(ZDOExtraData.ConnectionType.Portal) != portalZDOs[0].m_uid) {
          ConnectPortals(portalZDOs[0], portalZDOs[1], sessionId);
        }
      } else {
        for (int i = 0; i < count; i++) {
          ZDO sourceZDO = portalZDOs[i];
          ZDOID targetZDOID = sourceZDO.GetConnectionZDOID(ZDOExtraData.ConnectionType.Portal);

          // Source portal has a target portal, target portal exists, portal tags match.
          if (targetZDOID != ZDOID.None
              && zdoManager.m_objectsByID.TryGetValue(targetZDOID, out ZDO targetZDO)
              && targetZDO.GetString(ZDOVars.s_tag, string.Empty) == portalTag) {
            continue;
          }

          int offset = Random.Range(1, count - 1);
          targetZDO = portalZDOs[(i + offset) % count];

          ConnectPortal(sourceZDO, targetZDO.m_uid, sessionId);
        }
      }
    }
  }

  static void DisconnectPortal(ZDO zdo, long sessionId) {
    zdo.SetOwner(sessionId);
    zdo.UpdateConnection(ZDOExtraData.ConnectionType.Portal, ZDOID.None);

    _zdosToForceSend.Add(zdo.m_uid);
  }

  static void ConnectPortal(ZDO sourceZDO, ZDOID targetZDOID, long sessionId) {
    sourceZDO.SetOwner(sessionId);
    sourceZDO.SetConnection(ZDOExtraData.ConnectionType.Portal, targetZDOID);

    _zdosToForceSend.Add(sourceZDO.m_uid);
  }

  static void ConnectPortals(ZDO sourceZDO, ZDO targetZDO, long sessionId) {
    sourceZDO.SetOwner(sessionId);
    sourceZDO.SetConnection(ZDOExtraData.ConnectionType.Portal, targetZDO.m_uid);

    targetZDO.SetOwner(sessionId);
    targetZDO.SetConnection(ZDOExtraData.ConnectionType.Portal, sourceZDO.m_uid);

    _zdosToForceSend.Add(sourceZDO.m_uid);
    _zdosToForceSend.Add(targetZDO.m_uid);

    BetterServerPortals.LogInfo($"Connected portals: {sourceZDO.m_uid} <-> {targetZDO.m_uid}");
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
