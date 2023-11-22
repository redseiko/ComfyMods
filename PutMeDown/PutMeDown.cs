using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using ComfyLib;

using HarmonyLib;
using BepInEx.Configuration;

namespace PutMeDown {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class PutMeDown : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.putmedown";
    public const string PluginName = "PutMeDown";
    public const string PluginVersion = "1.0.0";

    static ManualLogSource _logger;
    Harmony _harmony;

    void Awake() {
      _logger = Logger;
      ComfyConfigUtils.BindConfig(Config);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    public static void LogInfo(object o) {
      _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {o}");
    }
  }
}
