namespace PutMeDown;

using System;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using ComfyLib;

using HarmonyLib;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class PutMeDown : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.putmedown";
  public const string PluginName = "PutMeDown";
  public const string PluginVersion = "1.3.0";

  static ManualLogSource _logger;

  void Awake() {
    _logger = Logger;
    ComfyConfigUtils.BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  public static void LogInfo(object obj) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
    Chat.m_instance.AddMessage(obj);
  }
}
