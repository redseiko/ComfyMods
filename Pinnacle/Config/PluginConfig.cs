using System;
using System.Collections.Generic;
using System.Linq;

using BepInEx.Configuration;

using ComfyLib;

using HarmonyLib;

using TMPro;

using UnityEngine;

namespace Pinnacle {
  public static class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }
    public static ConfigEntry<float> CenterMapLerpDuration { get; private set; }

    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      CenterMapLerpDuration =
          config.BindInOrder(
              "CenterMap",
              "lerpDuration",
              1f,
              "Duration (in seconds) for the CenterMap lerp.",
              new AcceptableValueRange<float>(0f, 3f));

      BindPinListPanelConfig(config);
      BindPinEditPanelConfig(config);
      BindPinFilterPanelConfig(config);

      _fejdStartupBindConfigQueue.Clear();
      _fejdStartupBindConfigQueue.Enqueue(() => BindMinimapConfig(config));
    }

    public static ConfigEntry<KeyboardShortcut> PinListPanelToggleShortcut { get; private set; }
    public static ConfigEntry<bool> PinListPanelShowPinPosition { get; private set; }

    public static ConfigEntry<Vector2> PinListPanelPosition { get; private set; }
    public static ConfigEntry<Vector2> PinListPanelSizeDelta { get; private set; }
    public static ConfigEntry<Color> PinListPanelBackgroundColor { get; private set; }

    public static void BindPinListPanelConfig(ConfigFile config) {
      PinListPanelToggleShortcut =
          config.BindInOrder(
              "PinListPanel",
              "pinListPanelToggleShortcut",
              new KeyboardShortcut(KeyCode.Tab),
              "Keyboard shortcut to toggle the PinListPanel on/off.");

      PinListPanelShowPinPosition =
          config.BindInOrder(
              "PinListPanel",
              "pinListPanelShowPinPosition",
              true,
              "Show the Pin.Position columns in the PineListPanel.");

      PinListPanelPosition =
          config.BindInOrder(
              "PinListPanel.Panel",
              "pinListPanelPosition",
              new Vector2(25f, 0f),
              "The value for the PinListPanel.Panel position (relative to pivot/anchors).");

      PinListPanelSizeDelta =
          config.BindInOrder(
              "PinListPanel.Panel",
              "pinListPanelSizeDelta",
              new Vector2(400f, 400f),
              "The value for the PinListPanel.Panel sizeDelta (width/height in pixels).");

      PinListPanelBackgroundColor =
          config.BindInOrder(
              "PinListPanel.Panel",
              "pinListPanelBackgroundColor",
              new Color(0f, 0f, 0f, 0.9f),
              "The value for the PinListPanel.Panel background color.");
    }

    public static ConfigEntry<float> PinEditPanelToggleLerpDuration { get; private set; }

    public static void BindPinEditPanelConfig(ConfigFile config) {
      PinEditPanelToggleLerpDuration =
          config.BindInOrder(
              "PinEditPanel.Toggle",
              "pinEditPanelToggleLerpDuration",
              0.25f,
              "Duration (in seconds) for the PinEdiPanl.Toggle on/off lerp.",
              new AcceptableValueRange<float>(0f, 3f));
    }

    public static ConfigEntry<Vector2> PinFilterPanelPosition { get; private set; }
    public static ConfigEntry<float> PinFilterPanelGridIconSize { get; private set; }

    public static void BindPinFilterPanelConfig(ConfigFile config) {
      PinFilterPanelPosition =
          config.BindInOrder(
              "PinFilterPanel.Panel",
              "pinFilterPanelPanelPosition",
              new Vector2(-25f, 0f),
              "The value for the PinFilterPanel.Panel position (relative to pivot/anchors).");

      PinFilterPanelGridIconSize =
          config.BindInOrder(
              "PinFilterPanel.Grid",
              "pinFilterPanelGridIconSize",
              30f,
              "The size of the PinFilterPanel.Grid icons.",
              new AcceptableValueRange<float>(10f, 100f));
    }

    public static ConfigEntry<string> PinFont { get; private set; }
    public static ConfigEntry<int> PinFontSize { get; private set; }

    public static ConfigEntry<KeyboardShortcut> AddPinAtMouseShortcut { get; private set; }

    public static void BindMinimapConfig(ConfigFile config) {
      PinFont =
          config.BindInOrder(
              "Minimap",
              "Pin.Font",
              defaultValue: UIResources.ValheimNorseFont,
              "The font for the Pin text on the Minimap.",
              new AcceptableValueList<string>(
              Resources.FindObjectsOfTypeAll<TMP_FontAsset>().Select(f => f.name).OrderBy(f => f).ToArray()));

      PinFontSize =
          config.BindInOrder(
              "Minimap",
              "Pin.FontSize",
              defaultValue: 18,
              "The font size for the Pin text on the Minimap.",
              new AcceptableValueRange<int>(2, 26));

      PinFont.OnSettingChanged(PinMarkerUtils.SetPinNameFont);
      PinFontSize.OnSettingChanged(PinMarkerUtils.SetPinNameFontSize);

      AddPinAtMouseShortcut =
          config.BindInOrder(
              "Minimap.Actions",
              "addPinAtMouseShortcut",
              KeyboardShortcut.Empty,
              "Keyboard shortcut to add a Minimap.Pin at the mouse position.");
    }

    static readonly Queue<Action> _fejdStartupBindConfigQueue = new();

    [HarmonyPatch(typeof(FejdStartup))]
    static class FejdStartupPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(FejdStartup.Awake))]
      static void AwakePostfix() {
        while (_fejdStartupBindConfigQueue.Count > 0) {
          _fejdStartupBindConfigQueue.Dequeue()?.Invoke();
        }
      }
    }
  }
}
