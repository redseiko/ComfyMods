namespace ComfyLib;

using System;
using System.Collections.Generic;
using System.Globalization;

using BepInEx.Logging;

public static class ComfyLogger {
  public static ManualLogSource CurrentLogger { get; private set; }

  public static void BindLogger(ManualLogSource logger) {
    CurrentLogger = logger;
  }

  static readonly Stack<Terminal> _contextStack = [];
  static Terminal _currentContext = default;

  public static void PushContext(Terminal context) {
    _contextStack.Push(context);
    _currentContext = context;
  }

  public static void PopContext() {
    _currentContext = _contextStack.Count > 0 ? _contextStack.Pop() : default;
  }

  public static void LogInfo(object obj, Terminal context = default) {
    LogMessage(LogLevel.Info, obj.ToString(), context);
  }

  public static void LogError(object obj, Terminal context = default) {
    LogMessage(LogLevel.Error, obj.ToString(), context);
  }

  static void LogMessage(LogLevel logLevel, string message, Terminal context) {
    CurrentLogger.Log(logLevel, $"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {message}");

    if (context) {
      context.AddString(message);
    } else if (_currentContext) {
      _currentContext.AddString(message);
    }
  }
}
