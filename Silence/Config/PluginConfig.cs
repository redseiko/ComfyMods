namespace Silence;

using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<KeyboardShortcut> ToggleSilenceShortcut { get; private set; }
  public static ConfigEntry<bool> HideChatWindow { get; private set; }
  public static ConfigEntry<bool> HideInWorldTexts { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable/disable this mod (restart required).");

    ToggleSilenceShortcut =
        config.BindInOrder(
            "Silence",
            "toggleSilenceShortcut",
            new KeyboardShortcut(KeyCode.S, KeyCode.RightControl),
            "Shortcut to toggle silence.");

    HideChatWindow =
        config.BindInOrder(
            "Silence",
            "hideChatWindow",
            true,
            "When silenced, chat window is hidden.");

    HideInWorldTexts =
        config.BindInOrder(
            "Silence",
            "hideInWorldTexts",
            true,
            "When silenced, hides text in-world.");
  }
}
