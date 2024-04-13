namespace EnRoute;

using System;
using System.Collections.Generic;
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
  public const string PluginVersion = "1.4.0";

  static ManualLogSource _logger;
  Harmony _harmony;

  void Awake() {
    _logger = Logger;

    BindConfig(Config);
    SetupNearbyRPCMethods();

    _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  void OnDestroy() {
    _harmony?.UnpatchSelf();
  }

  public static void SetupNearbyRPCMethods() {
    NearbyRPCMethodByHashCode.Clear();
    string[] names = NearbyRPCMethodNames.Value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

    foreach (string name in names) {
      NearbyRPCMethodByHashCode[name.GetStableHashCode()] = name;
    }

    NearbyRPCMethodHashCodes.Clear();
    NearbyRPCMethodHashCodes.UnionWith(NearbyRPCMethodByHashCode.Keys);

    LogInfo($"NearbyRPCMethods set to: {string.Join(", ", NearbyRPCMethodByHashCode.Values)}");
  }

  public static readonly Dictionary<int, string> NearbyRPCMethodByHashCode = new();
  public static readonly HashSet<int> NearbyRPCMethodHashCodes = new();

  public static long NetTimeTicks = 0L;

  public static void LogInfo(object obj) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }
}
