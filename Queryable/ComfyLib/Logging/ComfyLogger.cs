namespace ComfyLib;

using System;
using System.Globalization;

using BepInEx.Logging;

public static class ComfyLogger {
  public static ManualLogSource CurrentLogger { get; private set; }

  public static void BindLogger(ManualLogSource logger) {
    CurrentLogger = logger;
  }

  public static void LogInfo(object obj, Terminal context = default) {
    CurrentLogger.Log(LogLevel.Info, $"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");

    if (context) {
      context.AddString($"{obj}");
    }
  }

  public static void LogError(object obj, Terminal context = default) {
    CurrentLogger.Log(LogLevel.Error, $"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");

    if (context) {
      context.AddString($"{obj}");
    }
  }
}
