namespace ColorfulWards;

using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<KeyboardShortcut> ChangeWardColorShortcut { get; private set; }
  public static ExtendedColorConfigEntry TargetWardColor { get; private set; }
  public static ConfigEntry<bool> UseRadiusForVerticalCheck { get; private set; }
  public static ConfigEntry<bool> ShowChangeColorHoverText { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    ChangeWardColorShortcut =
        config.BindInOrder(
            "Hotkeys",
            "changeWardColorShortcut",
            new KeyboardShortcut(KeyCode.E, KeyCode.LeftShift),
            "Keyboard shortcut to change (or clear) the color of the hovered ward.");

    TargetWardColor =
        new(
            config,
            "Color",
            "targetWardColor",
            Color.cyan,
            "Target color to set the ward glow effect to.",
            colorPaletteKey: "targetWardColorPalette");

    UseRadiusForVerticalCheck =
        config.BindInOrder(
            "PrivateArea",
            "useRadiusForVerticalCheck",
            true,
            "Use the ward radius for access/permission checks vertically. Vanilla is infinite up/down.");

    ShowChangeColorHoverText =
        config.BindInOrder(
            "Hud",
            "showChangeColorHoverText",
            false,
            "Show the 'change color' text when hovering over a lightsoure.");
  }
}
