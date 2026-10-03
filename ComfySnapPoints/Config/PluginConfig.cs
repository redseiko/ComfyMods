namespace ComfySnapPoints;

using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<KeyboardShortcut> IterateSourceSnapPointsShortcut { get; private set; }
  public static ConfigEntry<KeyboardShortcut> IterateDestinationSnapPointsShortcut { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");
  }
}
