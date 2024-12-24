namespace ColorfulPalettes;

using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<KeyboardShortcut> ToggleConfigSelectShortcut { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    ToggleConfigSelectShortcut =
        config.BindInOrder(
            "ConfigSelect",
            "toggleConfigSelectShortcut",
            new KeyboardShortcut(KeyCode.S, KeyCode.RightShift),
            "Shortcut to toggle the ConfigSelect panel on/off.");
  }
}
