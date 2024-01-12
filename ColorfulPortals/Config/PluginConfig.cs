using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

namespace ColorfulPortals {
  public static class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }

    public static ConfigEntry<KeyboardShortcut> ChangePortalColorShortcut { get; private set; }

    public static ExtendedColorConfigEntry TargetPortalColor { get; private set; }

    public static ConfigEntry<bool> ShowChangeColorHoverText { get; private set; }

    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      ChangePortalColorShortcut =
          config.BindInOrder(
              "Hotkeys",
              "changePortalColorShortcut",
              new KeyboardShortcut(KeyCode.E, KeyCode.LeftShift),
              "Keyboard shortcut to change (or clear) the color of the hovered/targeted portal.");

      TargetPortalColor =
          new(
              config,
              "Color",
              "targetPortalColor",
              Color.cyan,
              "Target color to set any torch/fire to.",
              colorPaletteKey: "targetPortalColorPalette");

      ShowChangeColorHoverText =
          config.BindInOrder(
              "Hud", "showChangeColorHoverText", false, "Show the 'change color' text when hovering over a portal.");

      BindUpdateColorsConfig(config);
    }

    public static ConfigEntry<int> UpdateColorsFrameLimit { get; private set; }
    public static ConfigEntry<float> UpdateColorsWaitInterval { get; private set; }

    static void BindUpdateColorsConfig(ConfigFile config) {
      UpdateColorsFrameLimit =
          config.BindInOrder(
              "UpdateColors",
              "updateColorsFrameLimit",
              100,
              "Limit for how many TelepwortWorldColor.UpdateColors to process per update frame.",
              new AcceptableValueRange<int>(50, 250));

      UpdateColorsWaitInterval =
          config.BindInOrder(
              "UpdateColors",
              "updateColorsWaitInterval",
              5f,
              "Interval to wait after each TelepwortWorldColor.UpdateColors loop. *Restart required!*",
              new AcceptableValueRange<float>(0.5f, 10f));
    }
  }
}
