namespace Transporter;

using System;
using System.Globalization;
using System.IO;

using UnityEngine;

using static PluginConfig;

public static class AccessUtils {
  public static SyncedList AccessList { get; } =
      new SyncedList(
          Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), AccessListFilename.Value),
          "Transporter access list. One SteamId per line.");

  public static bool HasAccess(string steamId) {
    if (string.IsNullOrWhiteSpace(steamId)) {
      return false;
    }

    return AccessList.Contains(steamId) || ZNet.m_instance.m_adminList.Contains(steamId);
  }

  public static StreamWriter AccessLogWriter { get; } =
      File.AppendText(Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), AccessLogFilename.Value));

  public static void LogAccess(object obj) {
    AccessLogWriter.WriteLine($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
    AccessLogWriter.Flush();
  }

  public static void RegisterRPCs(ZNetPeer netPeer) {
    if (HasAccess(netPeer.m_socket.GetHostName())) {
      netPeer.m_rpc.Register<long, Vector3>("RequestTeleport", TeleportManager.RequestTeleport);
      netPeer.m_rpc.Register<ZDOID, Vector3>("RequestTeleportByZDOID", TeleportManager.RequestTeleportByZDOID);
    }
  }
}
