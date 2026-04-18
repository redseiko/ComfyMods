namespace SkyTree;

using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class SkyTree : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.skytree";
  public const string PluginName = "SkyTree";
  public const string PluginVersion = "1.6.0";

  static ManualLogSource _logger;

  void Awake() {
    BindConfig(Config);

    _logger = Logger;
    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  public static void LogInfo(object obj) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }

  public static void LogWarning(object obj) {
    _logger.LogWarning($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }
}
