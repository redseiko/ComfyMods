using BepInEx.Configuration;
using UnityEngine;

namespace ColorfulPieces {
  public class PluginConfig {
    public static ConfigEntry<bool> _isModEnabled;

    public static ConfigEntry<KeyboardShortcut> _changePieceColorShortcut;
    public static ConfigEntry<KeyboardShortcut> _clearPieceColorShortcut;
    public static ConfigEntry<KeyboardShortcut> _copyPieceColorShortcut;

    public static ConfigEntry<Color> _targetPieceColor;
    public static ConfigEntry<string> _targetPieceColorHex;
    public static ConfigEntry<float> _targetPieceEmissionColorFactor;
    public static ConfigEntry<bool> _showChangeRemoveColorPrompt;
    public static ConfigEntry<int> _colorPromptFontSize;

    public static void CreateConfig(ConfigFile config) {
      _isModEnabled = config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      _changePieceColorShortcut =
          config.Bind(
              "Hotkeys",
              "changePieceColorShortcut",
              new KeyboardShortcut(KeyCode.R, KeyCode.LeftShift),
              "Shortcut to change the color of the hovered piece.");

      _clearPieceColorShortcut =
          config.Bind(
              "Hotkeys",
              "clearPieceColorShortcut",
              new KeyboardShortcut(KeyCode.R, KeyCode.LeftAlt),
              "Shortcut to clear the color of the hovered piece.");

      _copyPieceColorShortcut =
          config.Bind(
              "Hotkeys",
              "copyPieceColorShortcut",
              new KeyboardShortcut(KeyCode.R, KeyCode.LeftControl),
              "Shortcut to copy the color of the hovered piece.");

      _targetPieceColor =
          config.Bind("Color", "targetPieceColor", Color.cyan, "Target color to set the piece material to.");

      _targetPieceColorHex =
          config.Bind(
              "Color",
              "targetPieceColorHex",
              $"#{ColorUtility.ToHtmlStringRGB(Color.cyan)}",
              "Target color to set the piece material to, in HTML hex form (alpha unsupported).");

      _targetPieceEmissionColorFactor =
          config.Bind(
              "Color",
              "targetPieceEmissionColorFactor",
              0.4f,
              new ConfigDescription(
                  "Factor to multiply the target color by and set as emission color.",
                  new AcceptableValueRange<float>(0f, 0.8f)));

      _showChangeRemoveColorPrompt =
          config.Bind("Hud", "showChangeRemoveColorPrompt", true, "Show the 'change/remove/copy' color text prompt.");

      _colorPromptFontSize =
          config.Bind("Hud", "colorPromptFontSize", 15, "Font size for the 'change/remove/copy' color text prompt.");
    }
  }
}
