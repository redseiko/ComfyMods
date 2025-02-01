namespace Entitlement;

using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Entitlement : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.entitlement";
  public const string PluginName = "Entitlement";
  public const string PluginVersion = "1.0.0";

  static ManualLogSource _logger;

  void Awake() {
    _logger = Logger;

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  public static void LogInfo(object obj) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }
}
