namespace PostalCode;

using System;
using System.Globalization;
using System.Reflection;

using BepInEx;

using HarmonyLib;
using BepInEx.Logging;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class PostalCode : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.postalcode";
  public const string PluginName = "PostalCode";
  public const string PluginVersion = "1.0.0";

  static ManualLogSource _logger;

  void Awake() {
    _logger = Logger;

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  public static void LogInfo(object obj) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }

  public static void LogError(object obj) {
    _logger.LogError($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }
}
