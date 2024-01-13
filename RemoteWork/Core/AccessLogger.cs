using System;
using System.Globalization;
using System.IO;

using static RemoteWork.PluginConfig;

namespace RemoteWork {
  public static class AccessLogger {
    static StreamWriter _logWriter;

    public static StreamWriter LogWriter {
      get {
        _logWriter ??=
            File.AppendText(Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), AccessLogFilename.Value));

        return _logWriter;
      }
    }

    public static void Log(string status, string steamId, string command) {
      LogWriter.WriteLine(
          $"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] [{status}] [{steamId}] {command}");
      LogWriter.Flush();

      RemoteWork.LogInfo($"[{status}] {steamId} executing remote command: {command}");
    }
  }
}
