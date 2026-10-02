namespace BetterZeeRouter;

using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class BetterZeeRouter : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.betterzeerouter";
  public const string PluginName = "BetterZeeRouter";
  public const string PluginVersion = "1.12.0";

  static ManualLogSource _logger;

  void Awake() {
    _logger = Logger;
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);

    RegisterStandardHandlers();
  }

  static void RegisterStandardHandlers() {
    IgnoreRPCHandler.Register(IgnoreRPCHandlerMethodNamesList.GetStringValues());
    SetTargetHandler.Register();
    TeleportPlayerHandler.Register();
  }

  public static void LogInfo(string message) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {message}");
  }
}
