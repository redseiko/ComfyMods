namespace Pinnacle;

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Pinnacle : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.pinnacle";
  public const string PluginName = "Pinnacle";
  public const string PluginVersion = "1.13.0";

  static ManualLogSource _logger;

  void Awake() {
    _logger = Logger;
    BindConfig(Config); 

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  public static void TogglePinnacle(bool toggleOn) {
    PinEditPanelController.TogglePanel(pinToEdit: null);
    PinListPanelController.TogglePanel(toggleOn: false);
    PinFilterPanelController.TogglePanel(toggleOn: toggleOn);
    ToggleVanillaIconPanels(toggleOn: !toggleOn);
  }

  public static void ToggleVanillaIconPanels(bool toggleOn) {
    foreach (
        GameObject panel in Minimap.m_instance.Ref()?.m_largeRoot
            .Children()
            .Where(child => child.name.StartsWith("IconPanel"))) {
      panel.SetActive(toggleOn);
    }
  }

  public static void Log(LogLevel logLevel, object o) {
    _logger.Log(logLevel, $"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {o}");
  }

  public static void LogInfo(object obj) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
    Chat.m_instance.AddMessage(obj);
  }

  public static void LogWarning(object obj) {
    _logger.LogWarning($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
    Chat.m_instance.AddMessage(obj);
  }
}
