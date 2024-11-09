﻿namespace Pinnacle;

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
  public const string PluginVersion = "1.11.0";

  static ManualLogSource _logger;

  void Awake() {
    _logger = Logger;
    BindConfig(Config); 

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  public static void TogglePinnacle(bool toggleOn) {
    TogglePinEditPanel(pinToEdit: null);
    PinListPanelController.TogglePanel(toggleOn: false);
    TogglePinFilterPanel(toggleOn: toggleOn);
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

  public static PinEditPanel PinEditPanel { get; private set; }

  public static void TogglePinEditPanel(Minimap.PinData pinToEdit = null) {
    if (!PinEditPanel?.Panel) {
      PinEditPanel = new(Minimap.m_instance.m_largeRoot.transform);
      PinEditPanel.Panel.RectTransform()
          .SetAnchorMin(new(0.5f, 0f))
          .SetAnchorMax(new(0.5f, 0f))
          .SetPivot(new(0.5f, 0f))
          .SetPosition(new(0f, 25f))
          .SetSizeDelta(new(200f, 200f));
    }

    if (pinToEdit == null) {
      PinEditPanel.SetTargetPin(null);
      PinEditPanel.SetActive(false);
    } else {
      CenterMapHelper.CenterMapOnPosition(pinToEdit.m_pos);

      PinEditPanel.SetTargetPin(pinToEdit);
      PinEditPanel.SetActive(true);
    }
  }

  public static PinFilterPanel PinFilterPanel { get; private set; }

  public static void TogglePinFilterPanel(bool toggleOn) {
    if (!PinFilterPanel?.Panel) {
      PinFilterPanel = new(Minimap.m_instance.m_largeRoot.transform);
      PinFilterPanel.Panel.RectTransform()
          .SetAnchorMin(new(1f, 0.5f))
          .SetAnchorMax(new(1f, 0.5f))
          .SetPivot(new(1f, 0.5f))
          .SetPosition(PinFilterPanelPosition.Value);

      PinFilterPanelGridIconSize.OnSettingChanged(PinFilterPanel.SetPanelStyle);
      PinFilterPanel.PanelDragger.OnPanelEndDrag += (_, position) => PinFilterPanelPosition.Value = position;
    }

    PinFilterPanel.Panel.SetActive(toggleOn);
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
