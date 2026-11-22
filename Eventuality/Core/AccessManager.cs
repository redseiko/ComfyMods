namespace Eventuality;

using System;
using System.IO;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public static class AccessManager {
  public static SyncedAuthList AccessAuthList { get; private set; } =
      new SyncedAuthList(
          Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), AccessListFilename.Value),
          "Allowed to start/end/configure RandomEventSystem on the server. One SteamId per line.");

  public static FileLogWriter AccessLogWriter { get; private set; } =
      new FileLogWriter(
          Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), AccessLogFilename.Value),
          "timestamp,status,senderId,hostName,eventName,posX,posY,posZ");

  public static bool HasAccess(string hostName) {
    return !string.IsNullOrWhiteSpace(hostName) && AccessAuthList.Contains(hostName);
  }

  public static void LogAccess(long senderId, string hostName, string status, string eventName, Vector3 position) {
    long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    AccessLogWriter.WriteLine(
        $"{timestamp},{status},{senderId},{hostName},{eventName},{position.x:F0},{position.y:F0},{position.z:F0}");
  }
}
