using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

namespace HoldMyMead {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class HoldMyMead : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.holdmymead";
    public const string PluginName = "HoldMyMead";
    public const string PluginVersion = "1.0.0";

    static ManualLogSource _logger;
    Harmony _harmony;

    void Awake() {
      _logger = Logger;

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
