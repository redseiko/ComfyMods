namespace Atlas;

using System;
using System.Collections.Generic;

using ComfyLib;

public static class ZDOManUtils {
  public static void SetTimeCreatedFields(ZDO zdo) {
    if (!zdo.HasLong(Atlas.TimeCreatedHash)) {
      ZDOExtraData.Set(zdo.m_uid, Atlas.TimeCreatedHash, TimeSpan.TicksPerSecond);
      ZDOExtraData.Set(zdo.m_uid, Atlas.EpochTimeCreatedHash, 1L);
    }
  }

  public static void SetTimeCreatedDelegate(ZDO zdo) {
    if (!zdo.HasLong(Atlas.TimeCreatedHash)) {
      zdo.Set(Atlas.TimeCreatedHash, (long) (ZNet.m_instance.m_netTime * TimeSpan.TicksPerSecond));
      zdo.Set(Atlas.EpochTimeCreatedHash, DateTimeOffset.Now.ToUnixTimeSeconds());
    }

    if (!zdo.HasZDOID(Atlas.OriginalUidUserIdHash, Atlas.OriginalUidIdHash)) {
      zdo.SetZDOID(Atlas.OriginalUidUserIdHash, Atlas.OriginalUidIdHash, zdo.m_uid);
    }
  }

  public static void SetTimeCreatedNewZDO(ZDOID uid) {
    ZDOExtraData.Set(uid, Atlas.TimeCreatedHash, (long) (ZNet.m_instance.m_netTime * TimeSpan.TicksPerSecond));
    ZDOExtraData.Set(uid, Atlas.EpochTimeCreatedHash, DateTimeOffset.Now.ToUnixTimeSeconds());

    ZDOExtraData.Set(uid, Atlas.OriginalUidUserIdHash, uid.UserID);
    ZDOExtraData.Set(uid, Atlas.OriginalUidIdHash, uid.ID);
  }

  public static void ConnectSpawners(ZDOMan zdoManager) {
    PluginLogger.LogInfo($"Starting ConnectSpawners with caching.");

    Dictionary<ZDOID, ZDOConnectionHashData> spawned = [];
    Dictionary<int, ZDOID> targetsByHash = [];

    foreach (KeyValuePair<ZDOID, ZDOConnectionHashData> pair in ZDOExtraData.s_connectionsHashData) {
      if (pair.Value.m_type == ZDOExtraData.ConnectionType.Spawned) {
        spawned.Add(pair.Key, pair.Value);
      } else if (pair.Value.m_type == (ZDOExtraData.ConnectionType.Spawned | ZDOExtraData.ConnectionType.Target)) {
        targetsByHash[pair.Value.m_hash] = pair.Key;
      }
    }

    PluginLogger.LogInfo($"Connecting {spawned.Count} spawners against {targetsByHash.Count} targets.");

    Dictionary<ZDOID, ZDO> objectsById = zdoManager.m_objectsByID;
    long sessionId = zdoManager.m_sessionID;
    int connectedCount = 0;
    int doneCount = 0;

    foreach (KeyValuePair<ZDOID, ZDOConnectionHashData> pair in spawned) {
      if (pair.Key.IsNone() || !objectsById.TryGetValue(pair.Key, out ZDO zdo) || zdo == null) {
        continue;
      }

      zdo.SetOwner(sessionId);

      if (targetsByHash.TryGetValue(pair.Value.m_hash, out ZDOID targetZDOID) && pair.Key != targetZDOID) {
        connectedCount++;
        zdo.SetConnection(ZDOExtraData.ConnectionType.Spawned, targetZDOID);
        targetsByHash.Remove(pair.Value.m_hash);
      } else {
        doneCount++;
        zdo.SetConnection(ZDOExtraData.ConnectionType.Spawned, ZDOID.None);
      }
    }

    foreach (ZDOID unmatchedTarget in targetsByHash.Values) {
      ZDOExtraData.RemoveConnectionHashData(
          unmatchedTarget, ZDOExtraData.ConnectionType.Spawned | ZDOExtraData.ConnectionType.Target);
    }

    PluginLogger.LogInfo($"Connected {connectedCount} spawners ({doneCount} 'done').");
  }
}
