namespace EnRoute;

using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class EnRoute : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.enroute";
  public const string PluginName = "EnRoute";
  public const string PluginVersion = "1.5.0";

  static ManualLogSource _logger;

  void Awake() {
    _logger = Logger;

    BindConfig(Config);
    EnRouteManager.SetupNearbyRPCMethods();

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  public static void LogInfo(object obj) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }
}
