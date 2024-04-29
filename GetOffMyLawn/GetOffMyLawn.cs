namespace GetOffMyLawn;

using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public sealed class GetOffMyLawn : BaseUnityPlugin {
  public const string PluginGUID = "redseiko.valheim.getoffmylawn";
  public const string PluginName = "GetOffMyLawn";
  public const string PluginVersion = "1.8.1";

  static ManualLogSource _logger;
  Harmony _harmony;

  void Awake() {
    _logger = Logger;
    BindConfig(Config);

    _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
  }

  void OnDestroy() {
    _harmony?.UnpatchSelf();
  }

  public static void LogInfo(object obj) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }

  public static void LogWarning(object obj) {
    _logger.LogWarning($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }
}
