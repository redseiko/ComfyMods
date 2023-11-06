using System;
using System.Globalization;

using BepInEx.Logging;

namespace Atlas {
  public static class PluginLogger {
    public static ManualLogSource Logger { get; private set; }

    public static void BindLogger(ManualLogSource logger) {
      Logger = logger;
    }

    public static ManualLogSource LogInfo(string message) {
      Logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {message}");
      return Logger;
    }

    public static ManualLogSource LogWarning(string message) {
      Logger.LogWarning($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {message}");
      return Logger;
    }

    public static ManualLogSource LogError(string message) {
      Logger.LogError($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {message}");
      return Logger;
    }
  }
}
