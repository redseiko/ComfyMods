namespace OdinSaves;

using BepInEx.Configuration;

public static class PluginConfig {
  public static ConfigFile CurrentConfig { get; private set; }

  public static ConfigEntry<bool> IsModEnabled { get; private set; }
  public static ConfigEntry<int> SavePlayerProfileInterval { get; private set; }
  public static ConfigEntry<bool> SetLogoutPointOnSave { get; private set; }
  public static ConfigEntry<bool> ShowMessageOnModSave { get; private set; }

  public static ConfigEntry<bool> EnableMapDataCompression { get; private set; }

  public static void BindConfig(ConfigFile config) {
    CurrentConfig = config;

    IsModEnabled =
        config.Bind(
            "Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    SavePlayerProfileInterval =
        config.Bind(
            "Global",
            "savePlayerProfileInterval",
            300,
            new ConfigDescription(
                "Interval (seconds) for how often to save the player profile. Game default (and maximum) is 1200s.",
                new AcceptableValueRange<int>(5, 1200)));

    SetLogoutPointOnSave =
        config.Bind(
            "Global",
            "setLogoutPointOnSave",
            true,
            "Sets your logout point to your current position when the mod performs a save.");

    ShowMessageOnModSave =
        config.Bind(
            "Global",
            "saveMessageOnModSave",
            true,
            "Show a message (in the middle of your screen) when the mod tries to save.");

    EnableMapDataCompression =
        config.Bind(
            "MapData.Compression",
            "enableMapDataCompression",
            false,
            "Enables the MapData compression feature on the character select screen (restart required).");
  }
}
