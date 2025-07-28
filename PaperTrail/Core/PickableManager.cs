namespace PaperTrail;

using System.IO;

using UnityEngine;

public static class PickableManager {
  static StreamWriter _rpcPickLog = default;

  public static void Initialize() {    
    _rpcPickLog ??=
        File.AppendText(
            Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), "paper-trail-rpc-pick-log.txt"));

    _rpcPickLog.AutoFlush = true;
  }

  public static void LogRPCPick(long senderPeerId, long targetPeerId, ZDOID targetZDOID) {
    if (targetZDOID == ZDOID.None
        || !ZDOMan.s_instance.m_objectsByID.TryGetValue(targetZDOID, out ZDO targetZDO)) {
      return;
    }

    Vector3 targetPosition = targetZDO.m_position;

    string logMessage =
        $"{System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()},"
            + $"{senderPeerId},{targetPeerId},\"{targetZDOID}\","
            + $"{targetZDO.m_prefab},\"{targetPosition.x:F0},{targetPosition.y:F0},{targetPosition.z:F0}\","
            + $"{targetZDO.GetInt(ZDOVars.s_picked, -1)},{targetZDO.GetLong(ZDOVars.s_pickedTime, -1)},"
            + $"{targetZDO.GetInt(ZDOVars.s_enabled, -1)}";

    _rpcPickLog.WriteLine(logMessage);
    PaperTrail.LogInfo(logMessage);
  }  
}
