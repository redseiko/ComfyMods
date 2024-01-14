using System;
using System.Globalization;
using System.IO;

using static Transporter.PluginConfig;

namespace Transporter {
  public static class AccessUtils {
    public static SyncedList AccessList { get; } =
        new SyncedList(
            Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), AccessListFilename.Value),
            "Transporter access list. One SteamId per line.");

    public static bool HasAccess(string steamId) {
      return AccessList.Contains(steamId) || ZNet.m_instance.m_adminList.Contains(steamId);
    }

    public static StreamWriter AccessLogWriter { get; } =
        File.AppendText(Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), AccessLogFilename.Value));

    public static void LogAccess(object obj) {
      AccessLogWriter.WriteLine($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
      AccessLogWriter.Flush();
    }
  }
}
