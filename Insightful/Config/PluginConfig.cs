namespace Insightful;

using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }
  public static ConfigEntry<KeyboardShortcut> ReadHiddenTextShortcut { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    ReadHiddenTextShortcut =
        config.BindInOrder(
            "Hotkeys",
            "readHiddenTextShortcut",
            new KeyboardShortcut(KeyCode.R, KeyCode.RightShift),
            "Shortcut to read hidden text inscriptions embedded within objects.");
  }
}
