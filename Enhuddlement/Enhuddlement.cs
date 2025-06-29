namespace Enhuddlement;

using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Enhuddlement : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.enhuddlement";
  public const string PluginName = "Enhuddlement";
  public const string PluginVersion = "1.4.0";

  public static Harmony HarmonyInstance { get; private set; }

  static ManualLogSource _logger;

  void Awake() {
    _logger = Logger;
    BindConfig(Config);

    HarmonyInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  public static void LogInfo(object obj) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }

  public static void LogError(object obj) {
    _logger.LogError($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }
}
