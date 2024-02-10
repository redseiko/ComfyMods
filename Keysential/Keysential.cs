namespace Keysential;

using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Keysential : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.keysential";
  public const string PluginName = "Keysential";
  public const string PluginVersion = "1.8.0";

  static ManualLogSource _logger;
  Harmony _harmony;

  void Awake() {
    _logger = Logger;
    BindConfig(Config);

    _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  void OnDestroy() {
    _harmony?.UnpatchSelf();
  }

  public static void LogInfo(object o) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {o}");
  }

  public static void LogWarning(object o) {
    _logger.LogWarning($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {o}");
  }

  public static void LogError(object o) {
    _logger.LogError($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {o}");
  }
}
