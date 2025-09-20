namespace ComfySigns;

using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using ComfyLib;

using HarmonyLib;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class ComfySigns : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.comfysigns";
  public const string PluginName = "ComfySigns";
  public const string PluginVersion = "1.11.0";

  static ManualLogSource _logger;

  void Awake() {
    _logger = Logger;
    ComfyConfigUtils.BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  public static void LogInfo(object obj) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }

  public static void LogWarning(object obj) {
    _logger.LogWarning($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
  }
}
