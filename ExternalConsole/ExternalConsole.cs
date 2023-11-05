using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using static ExternalConsole.PluginConfig;

namespace ExternalConsole {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class ExternalConsole : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.externalconsole";
    public const string PluginName = "ExternalConsole";
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

    public static void LogInfo(object data) {
      _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {data}");
    }
  }
}
