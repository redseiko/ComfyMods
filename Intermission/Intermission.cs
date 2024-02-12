namespace Intermission;

using System;
using System.Globalization;
using System.IO;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Intermission : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.intermission";
  public const string PluginName = "Intermission";
  public const string PluginVersion = "1.4.0";

  static ManualLogSource _logger;
  Harmony _harmony;

  void Awake() {
    _logger = Logger;
    BindConfig(Config);

    if (IsModEnabled.Value) {
      CustomAssets.Initialize(Path.Combine(Path.GetDirectoryName(Config.ConfigFilePath), PluginName));
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }
  }

  void OnDestroy() {
    _harmony?.UnpatchSelf();
  }

  public static void LogInfo(object o) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {o}");
  }

  public static void LogError(object o) {
    _logger.LogError($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {o}");
  }
}
