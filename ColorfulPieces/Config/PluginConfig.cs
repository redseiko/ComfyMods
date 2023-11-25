using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

namespace ColorfulPieces {
  public class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }

    public static ConfigEntry<KeyboardShortcut> ChangePieceColorShortcut { get; private set; }
    public static ConfigEntry<KeyboardShortcut> ClearPieceColorShortcut { get; private set; }
    public static ConfigEntry<KeyboardShortcut> CopyPieceColorShortcut { get; private set; }

    public static ExtendedColorConfigEntry TargetPieceColor { get; private set; }
    public static ConfigEntry<float> TargetPieceEmissionColorFactor { get; private set; }

    public static ConfigEntry<bool> ShowChangeRemoveColorPrompt { get; private set; }
    public static ConfigEntry<int> ColorPromptFontSize { get; private set; }

    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      IsModEnabled.OnSettingChanged(ComfyCommandUtils.ToggleCommands);

      ChangePieceColorShortcut =
          config.Bind(
              "Hotkeys",
              "changePieceColorShortcut",
              new KeyboardShortcut(KeyCode.R, KeyCode.LeftShift),
              "Shortcut to change the color of the hovered piece.");

      ClearPieceColorShortcut =
          config.Bind(
              "Hotkeys",
              "clearPieceColorShortcut",
              new KeyboardShortcut(KeyCode.R, KeyCode.LeftAlt),
              "Shortcut to clear the color of the hovered piece.");

      CopyPieceColorShortcut =
          config.Bind(
              "Hotkeys",
              "copyPieceColorShortcut",
              new KeyboardShortcut(KeyCode.R, KeyCode.LeftControl),
              "Shortcut to copy the color of the hovered piece.");

      TargetPieceColor =
          new(
              config,
              "Color",
              "targetPieceColor",
              Color.cyan,
              "Target color to set the piece material to.",
              "targetPieceColorPalette");

      // Lock alpha to 1f as ZDO only saves as Vector3 (RGB) value for now.
      TargetPieceColor.AlphaInput.SetValueRange(1f, 1f);

      TargetPieceEmissionColorFactor =
          config.Bind(
              "Color",
              "targetPieceEmissionColorFactor",
              0.4f,
              new ConfigDescription(
                  "Factor to multiply the target color by and set as emission color.",
                  new AcceptableValueRange<float>(0f, 0.8f)));

      ShowChangeRemoveColorPrompt =
          config.Bind("Hud", "showChangeRemoveColorPrompt", false, "Show the 'change/remove/copy' color text prompt.");

      ColorPromptFontSize =
          config.Bind("Hud", "colorPromptFontSize", 15, "Font size for the 'change/remove/copy' color text prompt.");

      BindUpdateColorsConfig(config);
      BindPieceStabilityColorsConfig(config);
    }

    public static ConfigEntry<int> UpdateColorsFrameLimit { get; private set; }
    public static ConfigEntry<float> UpdateColorsWaitInterval { get; private set; }

    static void BindUpdateColorsConfig(ConfigFile config) {
      UpdateColorsFrameLimit =
          config.BindInOrder(
              "UpdateColors",
              "updateColorsFrameLimit",
              100,
              "Limit for how many PieceColor.UpdateColors to process per update frame.",
              new AcceptableValueRange<int>(50, 250));

      UpdateColorsWaitInterval =
          config.BindInOrder(
              "UpdateColors",
              "updateColorsWaitInterval",
              5f,
              "Interval to wait after each PieceColor.UpdateColors loop. *Restart required!*",
              new AcceptableValueRange<float>(0.5f, 10f));
    }

    public static ExtendedColorConfigEntry PieceStabilityMinColor { get; private set; }
    public static ExtendedColorConfigEntry PieceStabilityMaxColor { get; private set; }

    static void BindPieceStabilityColorsConfig(ConfigFile config) {
      PieceStabilityMinColor =
          new(
              config,
              "PieceStabilityColors",
              "pieceStabilityMinColor",
              Color.red,
              "Color for the Piece Stability highlighting gradient to use for minimum stability.");

      PieceStabilityMaxColor =
          new(
              config,
              "PieceStabilityColors",
              "pieceStabilityMaxColor",
              Color.green,
              "Color for the Piece Stability highlighting gradient to use for maximum stability.");
    }
  }
}
