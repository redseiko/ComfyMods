namespace PaperTrail;

using System.Collections.Generic;
using System.IO;

using UnityEngine;

public static class PickableManager {
  static StreamWriter _rpcPickLog = default;

  public static void Initialize() {
    if (_rpcPickLog == default) {
      _rpcPickLog =
          File.AppendText(
              Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), "paper-trail-rpc-log.txt"));

      _rpcPickLog.AutoFlush = true;

      PickHandler.Register();
      SetPickedHandler.Register();
    }
  }

  public static readonly HashSet<int> PickablePrefabsToLog = [];

  public static void SetPickablePrefabsToLog(IEnumerable<string> prefabs) {
    PickablePrefabsToLog.Clear();

    foreach (string prefab in prefabs) {
      PickablePrefabsToLog.Add(prefab.GetStableHashCode());
    }

    PaperTrail.LogInfo($"Logging RPCs for {PickablePrefabsToLog.Count} Pickable prefabs.");
  }

  public static void LogRPCPick(long senderPeerId, long targetPeerId, ZDOID targetZDOID, int bonus) {
    if (targetZDOID == ZDOID.None
        || !ZDOMan.s_instance.m_objectsByID.TryGetValue(targetZDOID, out ZDO targetZDO)
        || !PickablePrefabsToLog.Contains(targetZDO.m_prefab)) {
      return;
    }

    Vector3 targetPosition = targetZDO.m_position;

    string logMessage =
        $"{System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()},"
            + $"RPC_Pick,"
            + $"{targetZDO.m_prefab},"
            + $"{senderPeerId},{targetPeerId},{targetZDOID},"
            + $"{targetPosition.x:F0},{targetPosition.y:F0},{targetPosition.z:F0},"
            + $"{bonus}";

    _rpcPickLog.WriteLine(logMessage);
    PaperTrail.LogInfo(logMessage);
  }

  public static void LogRPCSetPicked(long senderPeerId, long targetPeerId, ZDOID targetZDOID, bool picked) {
    if (targetZDOID == ZDOID.None
        || !ZDOMan.s_instance.m_objectsByID.TryGetValue(targetZDOID, out ZDO targetZDO)
        || !PickablePrefabsToLog.Contains(targetZDO.m_prefab)) {
      return;
    }

    Vector3 targetPosition = targetZDO.m_position;

    string logMessage =
        $"{System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()},"
            + $"RPC_SetPicked,"
            + $"{targetZDO.m_prefab},"
            + $"{senderPeerId},{targetPeerId},{targetZDOID},"
            + $"{targetPosition.x:F0},{targetPosition.y:F0},{targetPosition.z:F0},"
            + $"{picked}";

    _rpcPickLog.WriteLine(logMessage);
    PaperTrail.LogInfo(logMessage);
  }
}
