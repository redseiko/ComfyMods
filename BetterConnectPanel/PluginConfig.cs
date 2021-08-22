using BepInEx.Configuration;

using UnityEngine;

namespace BetterConnectPanel {
  internal class PluginConfig {
    internal static ConfigEntry<bool> _isModEnabled;
    internal static ConfigEntry<KeyboardShortcut> _networkPanelToggleShortcut;
    internal static ConfigEntry<Vector2> _networkPanelPosition;
    internal static ConfigEntry<int> _networkPanelFontSize;
    internal static ConfigEntry<Color> _networkPanelBackgroundColor;

    internal static void CreateConfig(ConfigFile config) {
      _isModEnabled =
          config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      _networkPanelToggleShortcut =
          config.Bind(
              "NetworkPanel",
              "networkPanelToggleShortcut",
              new KeyboardShortcut(KeyCode.F2, KeyCode.LeftShift),
              "Keyboard shortcut to toggle the NetworkPanel on/off.");

      _networkPanelPosition =
          config.Bind(
              "NetworkPanel", "networkPanelPosition", new Vector2(10f, -150f), "Position of the NetworkPanel.");

      _networkPanelFontSize =
          config.Bind(
              "NetworkPanel",
              "networkPanelFontSize",
              14,
              new ConfigDescription("Font size for the NetworkPanel.", new AcceptableValueRange<int>(6, 32)));

      _networkPanelBackgroundColor =
          config.Bind(
              "NetworkPanel",
              "networkPanelBackgroundColor",
              (Color) new Color32(0, 0, 0, 96),
              "Background color of the NetworkPanel.");
    }
  }
}
