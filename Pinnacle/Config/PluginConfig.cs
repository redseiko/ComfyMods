namespace Pinnacle;

using System;
using System.Collections.Generic;
using System.Linq;

using BepInEx.Configuration;

using ComfyLib;

using HarmonyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public static class PluginConfig {
  public static ConfigFile CurrentConfig { get; private set; }

  public static ConfigEntry<bool> IsModEnabled { get; private set; }
  public static ConfigEntry<float> CenterMapLerpDuration { get; private set; }

  public static void BindConfig(ConfigFile config) {
    CurrentConfig = config;

    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    IsModEnabled.OnSettingChanged(PinnacleUtils.TogglePinnacle);
    IsModEnabled.OnSettingChanged(ComfyCommandUtils.ToggleCommands);

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
    BindPinIconConfig(config);
    BindQuickMapPinConfig(config);

    LateBindConfigQueue.Reset(config);
    LateBindConfigQueue.Enqueue(BindMinimapConfig);
  }

  public static ConfigEntry<KeyboardShortcut> PinListPanelToggleShortcut { get; private set; }
  public static ConfigEntry<bool> PinListPanelShowPinPosition { get; private set; }

  public static ConfigEntry<Vector2> PinListPanelPosition { get; private set; }
  public static ConfigEntry<Vector2> PinListPanelSizeDelta { get; private set; }
  public static ConfigEntry<Color> PinListPanelBackgroundColor { get; private set; }

  public static ConfigEntry<ScrollRect.MovementType> PinListPanelScrollRectMovementType { get; private set; }
  public static ConfigEntry<float> PinListPanelScrollRectScrollSensitivity { get; private set; }

  public static ConfigEntry<bool> PinListPanelEditPinOnRowClick { get; private set; }

  public static void BindPinListPanelConfig(ConfigFile config) {
    PinListPanelToggleShortcut =
        config.BindInOrder(
            "PinListPanel",
            "pinListPanelToggleShortcut",
            new KeyboardShortcut(KeyCode.Tab),
            "Keyboard shortcut to toggle the PinListPanel on/off.");

    PinListPanelShowPinPosition =
        config.BindInOrder(
            "PinListPanel.Columns",
            "pinListPanelShowPinPosition",
            true,
            "Show the Pin.Position columns in the PinListPanel.");

    PinListPanelPosition =
        config.BindInOrder(
            "PinListPanel.Panel",
            "pinListPanelPosition",
            new Vector2(25f, 0f),
            "The value for the PinListPanel.Panel position (relative to pivot/anchors).");

    PinListPanelPosition.OnSettingChanged(PinListPanelController.SetPanelPosition);

    PinListPanelSizeDelta =
        config.BindInOrder(
            "PinListPanel.Panel",
            "pinListPanelSizeDelta",
            new Vector2(400f, 400f),
            "The value for the PinListPanel.Panel sizeDelta (width/height in pixels).");

    PinListPanelSizeDelta.OnSettingChanged(PinListPanelController.SetPanelSize);

    PinListPanelBackgroundColor =
        config.BindInOrder(
            "PinListPanel.Panel",
            "pinListPanelBackgroundColor",
            new Color(0f, 0f, 0f, 0.9f),
            "The value for the PinListPanel.Panel background color.");

    PinListPanelBackgroundColor.OnSettingChanged(PinListPanelController.SetBackgroundColor);

    PinListPanelScrollRectMovementType =
        config.BindInOrder(
            "PinListPanel.ScrollRect",
            "movementType",
            ScrollRect.MovementType.Clamped,
            "Determines how the PinListPanel scrolling should behave.",
            new AcceptableValueEnumList<ScrollRect.MovementType>(
                ScrollRect.MovementType.Clamped,
                ScrollRect.MovementType.Elastic));

    PinListPanelScrollRectMovementType.OnSettingChanged(PinListPanelController.SetScrollRectMovementType);

    PinListPanelScrollRectScrollSensitivity =
        config.BindInOrder(
            "PinListPanel.ScrollRect",
            "scrollSensitivity",
            3600f,
            "PinListPanel scroll-sensitivity when using the mouse-wheel.",
            new AcceptableValueRange<float>(0f, 7200f));

    PinListPanelScrollRectScrollSensitivity.OnSettingChanged(PinListPanelController.SetScrollRectScrollSensitivity);

    PinListPanelEditPinOnRowClick =
        config.BindInOrder(
            "PinListPanel.Behaviour",
            "pinListPanelEditPinOnRowClick",
            true,
            "If set, will show the PinEditPanel when a row is selected in the PinListPanel.");
  }

  public static ConfigEntry<Vector2> PinEditPanelDefaultPosition { get; private set; }
  public static ConfigEntry<float> PinEditPanelToggleLerpDuration { get; private set; }

  public static void BindPinEditPanelConfig(ConfigFile config) {
    PinEditPanelDefaultPosition =
        config.BindInOrder(
            "PinEditPanel",
            "pinEditPanelDefaultPosition",
            new Vector2(0f, 25f),
            "PinEditPanel position, relative to the bottom-center of the Minimap.");

    PinEditPanelToggleLerpDuration =
        config.BindInOrder(
            "PinEditPanel.Toggle",
            "pinEditPanelToggleLerpDuration",
            0.25f,
            "Duration (in seconds) for the PinEditPanel.Toggle on/off lerp.",
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

    PinFilterPanelGridIconSize.OnSettingChanged(PinFilterPanelController.UpdateStyle);
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
            [.. Resources.FindObjectsOfTypeAll<TMP_FontAsset>().Select(f => f.name).OrderBy(f => f)]));

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

  public static ConfigEntry<bool> ProcessPinIconColorTags { get; private set; }
  public static ConfigEntry<bool> ProcessPinIconSpriteTags { get; private set; }
  public static ConfigEntry<bool> ProcessPinIconScaleTags { get; private set; }

  public static ConfigEntry<bool> StripPinIconColorTagText { get; private set; }
  public static ConfigEntry<bool> StripPinIconSpriteTagText { get; private set; }
  public static ConfigEntry<bool> StripPinIconScaleTagText { get; private set; }

  static void BindPinIconConfig(ConfigFile config) {
    ProcessPinIconColorTags =
        config.BindInOrder(
            "PinIcon.Tags",
            "processPinIconColorTags",
            true,
            "If set, will process pin-icon-color tags: [#ff00ff]");

    ProcessPinIconSpriteTags =
        config.BindInOrder(
            "PinIcon.Tags",
            "processPinIconSpriteTags",
            true,
            "If set, will process pin-icon-sprite tags: [:sprite_name]");

    ProcessPinIconScaleTags =
        config.BindInOrder(
            "PinIcon.Tags",
            "processPinIconScaleTags",
            true,
            "If set, will process pin-icon-scale tags: [150%]");

    StripPinIconColorTagText =
        config.BindInOrder(
            "PinIcon.Tags",
            "stripPinIconColorTagText",
            true,
            "If set, will strip pin-icon-color tags from the pin-text.");

    StripPinIconSpriteTagText =
        config.BindInOrder(
            "PinIcon.Tags",
            "stripPinIconSpriteTagText",
            true,
            "If set, will strip pin-icon-sprite tags from the pin-text.");

    StripPinIconScaleTagText =
        config.BindInOrder(
            "PinIcon.Tags",
            "stripPinIconScaleTagText",
            true,
            "If set, will strip pin-icon-scale tags from the pin-text.");
  }

  public static ConfigEntry<string> QuickMapPinDefaultName { get; private set; }
  public static ConfigEntry<Minimap.PinType> QuickMapPinDefaultPinType { get; private set; }
  public static ConfigEntry<KeyboardShortcut> QuickMapPinShortcut { get; private set; }

  static void BindQuickMapPinConfig(ConfigFile config) {
    QuickMapPinDefaultName =
        config.BindInOrder(
            "QuickMapPin",
            "quickMapPinDefaultName",
            "QuickPin",
            "Default pin-name to use for a quick-map-pin.");

    QuickMapPinDefaultPinType =
        config.BindInOrder(
            "QuickMapPin",
            "quickMapPinDefaultPinType",
            Minimap.PinType.Icon3,
            "Default pin-type to use for a quick-map-pin.");

    QuickMapPinShortcut =
        config.BindInOrder(
            "QuickMapPin",
            "quickMapPinShortcut",
            new KeyboardShortcut(KeyCode.None),
            "Shortcut to add a quick-map-pin at current position with the Minimap is open.");
  }
}

public static class LateBindConfigQueue {
  static ConfigFile _currentConfig = default;
  static readonly Queue<Action<ConfigFile>> _queue = new();

  public static void Reset(ConfigFile currentConfig) {
    _currentConfig = currentConfig;
    _queue.Clear();
  }

  public static void Enqueue(Action<ConfigFile> bindConfigAction) {
    _queue.Enqueue(bindConfigAction);
  }

  [HarmonyPatch(typeof(FejdStartup))]
  static class FejdStartupPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(FejdStartup.Awake))]
    static void AwakePostfix() {
      while (_queue.Count > 0) {
        _queue.Dequeue().Invoke(_currentConfig);
      }
    }
  }
}
