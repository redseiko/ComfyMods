using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using static Transporter.PluginConfig;

namespace Transporter {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public sealed class Transporter : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.transporter";
    public const string PluginName = "Transporter";
    public const string PluginVersion = "1.1.0";

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

    public static void LogError(object obj) {
      _logger.LogError($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
    }
  }
}
