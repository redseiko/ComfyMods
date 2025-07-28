namespace PaperTrail;

using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using BetterZeeRouter;

using HarmonyLib;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency(BetterZeeRouter.PluginGuid, BepInDependency.DependencyFlags.HardDependency)]
public sealed class PaperTrail : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.papertrail";
  public const string PluginName = "PaperTrail";
  public const string PluginVersion = "1.0.0";

  static ManualLogSource _logger;

  void Awake() {
    _logger = Logger;

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);

    PickHandler.Register();
  }

  public static void LogInfo(object obj) {
    _logger.Log(LogLevel.Info, $"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }

  public static void LogError(object obj) {
    _logger.Log(LogLevel.Error, $"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }
}
