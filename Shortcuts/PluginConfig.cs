using BepInEx.Configuration;

using UnityEngine;

namespace Shortcuts {
  public class PluginConfig {
    public static ConfigEntry<bool> _isModEnabled;

    public static ConfigEntry<KeyboardShortcut> _toggleConsoleShortcut;
    public static ConfigEntry<KeyboardShortcut> _toggleHudShortcut;
    public static ConfigEntry<KeyboardShortcut> _toggleConnectPanelShortcut;

    public static ConfigEntry<KeyboardShortcut> _takeScreenshotShortcut;
    public static ConfigEntry<KeyboardShortcut> _toggleMouseCaptureShortcut;

    public static ConfigEntry<KeyboardShortcut> _toggleDebugFlyShortcut;
    public static ConfigEntry<KeyboardShortcut> _toggleDebugNoCostShortcut;
    public static ConfigEntry<KeyboardShortcut> _debugKillAllShortcut;
    public static ConfigEntry<KeyboardShortcut> _debugRemoveDropsShortcut;

    public static ConfigEntry<KeyboardShortcut> _hotbarItem1Shortcut;
    public static ConfigEntry<KeyboardShortcut> _hotbarItem2Shortcut;
    public static ConfigEntry<KeyboardShortcut> _hotbarItem3Shortcut;
    public static ConfigEntry<KeyboardShortcut> _hotbarItem4Shortcut;
    public static ConfigEntry<KeyboardShortcut> _hotbarItem5Shortcut;
    public static ConfigEntry<KeyboardShortcut> _hotbarItem6Shortcut;
    public static ConfigEntry<KeyboardShortcut> _hotbarItem7Shortcut;
    public static ConfigEntry<KeyboardShortcut> _hotbarItem8Shortcut;

    public static void CreateConfig(ConfigFile config) {
      _isModEnabled =
          config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod (restart required).");

      _toggleConsoleShortcut =
          config.Bind(
              "Console",
              "toggleConsoleShortcut",
              new KeyboardShortcut(KeyCode.F5),
              "Shortcut to toggle the Console on/off.");

      _toggleHudShortcut =
          config.Bind(
              "Hud",
              "toggleHudShortcut",
              new KeyboardShortcut(KeyCode.F3, KeyCode.LeftControl),
              "Shortcut to toggle the Hud on/off.");

      _toggleConnectPanelShortcut =
          config.Bind(
              "ConnectPanel",
              "toggleConnectPanelShortcut",
              new KeyboardShortcut(KeyCode.F2),
              "Shortcut to toggle the ConnectPanel on/off.");

      _takeScreenshotShortcut =
          config.Bind(
              "GameCamera",
              "takeScreenshotShortcut",
              new KeyboardShortcut(KeyCode.F11),
              "Shortcut to take a screenshot.");

      _toggleMouseCaptureShortcut =
          config.Bind(
              "GameCamera",
              "toggleMouseCaptureShortcut",
              new KeyboardShortcut(KeyCode.F1, KeyCode.LeftControl),
              "Shortcut to toggle mouse capture from the GameCamera.");

      _toggleDebugFlyShortcut =
          config.Bind(
              "Debugmode",
              "toggleDebugFlyShortcut",
              new KeyboardShortcut(KeyCode.Z),
              "Shortcut to toggle flying when in debugmode.");

      _toggleDebugNoCostShortcut =
          config.Bind(
              "Debugmode",
              "toggleDebugNoCostShortcut",
              new KeyboardShortcut(KeyCode.B),
              "Shortcut to toggle no-cost building when in debugmode.");

      _debugKillAllShortcut =
          config.Bind(
              "Debugmode",
              "debugKillAllShortcut",
              new KeyboardShortcut(KeyCode.None),
              "Shortcut to kill/damage all mobs around player. Unbound by default.");

      _debugRemoveDropsShortcut =
          config.Bind(
              "Debugmode",
              "debugRemoveDropsShortcut",
              new KeyboardShortcut(KeyCode.None),
              "Shortcut to 'removedrops' command. Unbound by default.");

      _hotbarItem1Shortcut =
          config.Bind(
              "Hotbar",
              "hotbarItem1Shortcut",
              new KeyboardShortcut(KeyCode.Alpha1),
              "Shortcut for the first slot in the Hotbar.");

      _hotbarItem2Shortcut =
          config.Bind(
              "Hotbar",
              "hotbarItem2Shortcut",
              new KeyboardShortcut(KeyCode.Alpha2),
              "Shortcut for the second slot in the Hotbar.");

      _hotbarItem3Shortcut =
          config.Bind(
              "Hotbar",
              "hotbarItem3Shortcut",
              new KeyboardShortcut(KeyCode.Alpha3),
              "Shortcut for the third slot in the Hotbar.");

      _hotbarItem4Shortcut =
          config.Bind(
              "Hotbar",
              "hotbarItem4Shortcut",
              new KeyboardShortcut(KeyCode.Alpha4),
              "Shortcut for the fourth slot in the Hotbar.");

      _hotbarItem5Shortcut =
          config.Bind(
              "Hotbar",
              "hotbarItem5Shortcut",
              new KeyboardShortcut(KeyCode.Alpha5),
              "Shortcut for the fifth slot in the Hotbar.");

      _hotbarItem6Shortcut =
          config.Bind(
              "Hotbar",
              "hotbarItem6Shortcut",
              new KeyboardShortcut(KeyCode.Alpha6),
              "Shortcut for the sixth slot in the Hotbar.");

      _hotbarItem7Shortcut =
          config.Bind(
              "Hotbar",
              "hotbarItem7Shortcut",
              new KeyboardShortcut(KeyCode.Alpha7),
              "Shortcut for the seventh slot in the Hotbar.");

      _hotbarItem8Shortcut =
          config.Bind(
              "Hotbar",
              "hotbarItem8Shortcut",
              new KeyboardShortcut(KeyCode.Alpha8),
              "Shortcut for the eight slot in the Hotbar.");
    }
  }
}
