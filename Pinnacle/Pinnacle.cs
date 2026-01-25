namespace Pinnacle;

using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using ComfyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Pinnacle : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.pinnacle";
  public const string PluginName = "Pinnacle";
  public const string PluginVersion = "1.16.0";

  static ManualLogSource _logger;

  void Awake() {
    _logger = Logger;
    BindConfig(Config); 

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  public static void Log(LogLevel logLevel, object o) {
    _logger.Log(logLevel, $"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {o}");
  }

  public static void LogInfo(object obj) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
    Chat.m_instance.AddMessage(obj);
  }

  public static void LogWarning(object obj) {
    _logger.LogWarning($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
    Chat.m_instance.AddMessage(obj);
  }
}
