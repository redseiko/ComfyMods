namespace LicensePlate;

using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class LicensePlate : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.licenseplate";
  public const string PluginName = "LicensePlate";
  public const string PluginVersion = "1.3.0";

  public static readonly int ShipLicensePlateHashCode = "ShipLicensePlate".GetStableHashCode();
  public static readonly int VagonLicensePlateHashCode = "VagonLicensePlate".GetStableHashCode();
  public static readonly int LicensePlateLastSetByHashCode = "LicensePlateLastSetBy".GetStableHashCode();
  public static readonly Regex HtmlTagsRegex = new("<.*?>");

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
