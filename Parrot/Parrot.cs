namespace Parrot;

using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using BetterZeeRouter;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency(BetterZeeRouter.PluginGuid, BepInDependency.DependencyFlags.HardDependency)]
public sealed class Parrot : BaseUnityPlugin {
  public const string PluginGUID = "redseiko.valheim.parrot";
  public const string PluginName = "Parrot";
  public const string PluginVersion = "1.5.1";

  static ManualLogSource _logger;

  void Awake() {
    _logger = Logger;
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);

    ChatMessageHandler.Register();
    SayHandler.Register();
  }

  public static void LogInfo(string message) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {message}");
  }

  public static void LogError(string message) {
    _logger.LogError($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {message}");
  }
}
