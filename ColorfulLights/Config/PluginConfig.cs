using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

namespace ColorfulLights {
  public static class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }
    public static ConfigEntry<KeyboardShortcut> ChangeColorActionShortcut { get; private set; }

    public static ExtendedColorConfigEntry TargetFireplaceColor { get; private set; }

    public static ConfigEntry<bool> ShowChangeColorHoverText { get; private set; }
    public static ConfigEntry<int> ColorPromptFontSize { get; private set; }

    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      ChangeColorActionShortcut =
          config.BindInOrder(
              "Hotkeys",
              "changeColorActionShortcut",
              new KeyboardShortcut(KeyCode.E, KeyCode.LeftShift),
              "Keyboard shortcut to change (or clear) the color of the hovered torch/fire.");

      TargetFireplaceColor =
          new(
              config,
              "Color",
              "targetFireplaceColor",
              Color.cyan,
              "Target color to set any torch/fire to.",
              colorPaletteKey: "targetFireplaceColoPalette");

      ShowChangeColorHoverText =
          config.BindInOrder(
              "Hud",
              "showChangeColorHoverText",
              false,
              "Show the 'change color' text when hovering over a lightsoure.");

      ColorPromptFontSize =
          config.BindInOrder(
              "Hud",
              "colorPromptFontSize",
              16,
              "Font size for the 'change color' text prompt.",
              new AcceptableValueRange<int>(4, 24));
    }
  }
}
