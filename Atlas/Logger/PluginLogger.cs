namespace Atlas;

using System;
using System.Globalization;

using BepInEx.Logging;

public static class PluginLogger {
  public static ManualLogSource Logger { get; private set; }

  public static void BindLogger(ManualLogSource logger) {
    Logger = logger;
  }

  public static void LogInfo(object obj) {
    Logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }

  public static void LogWarning(object obj) {
    Logger.LogWarning($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }

  public static void LogError(object obj) {
    Logger.LogError($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }
}
