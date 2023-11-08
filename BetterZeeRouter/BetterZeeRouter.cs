using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using static BetterZeeRouter.PluginConfig;
using static BetterZeeRouter.RpcHashCodes;

namespace BetterZeeRouter {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class BetterZeeRouter : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.betterzeerouter";
    public const string PluginName = "BetterZeeRouter";
    public const string PluginVersion = "1.6.0";

    static ManualLogSource _logger;
    Harmony _harmony;

    RoutedRpcManager _routedRpcManager;
    TeleportPlayerHandler _teleportPlayerHandler;

    void Awake() {
      _logger = Logger;
      BindConfig(Config);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);

      _routedRpcManager = RoutedRpcManager.Instance;
      _routedRpcManager.AddHandler(WntHealthChangedHashCode, new WntHealthChangedHandler());
      _routedRpcManager.AddHandler(DamageTextHashCode, new DamageTextHandler());
      _routedRpcManager.AddHandler(RpcSetTargetHashCode, new SetTargetHandler());

      _teleportPlayerHandler = new();
      _routedRpcManager.AddHandler(RpcTeleportPlayerHashCode, _teleportPlayerHandler);
      _routedRpcManager.AddHandler(RpcTeleportToHashCode, _teleportPlayerHandler);
    }

    void OnDestroy() {
      _teleportPlayerHandler?.Dispose();
      _harmony?.UnpatchSelf();
    }

    public static void LogInfo(string message) {
      _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {message}");
    }
  }
}