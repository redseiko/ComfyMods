using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using static RemoteWork.PluginConfig;

namespace RemoteWork {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public sealed class RemoteWork : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.remotework";
    public const string PluginName = "RemoteWork";
    public const string PluginVersion = "1.0.0";

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

    public static void LogInfo(object obj) {
      _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
    }
  }
}
