namespace Atlas;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<bool> IgnoreGenerateLocationsIfNeeded { get; private set; }
  public static ConfigEntry<bool> IgnoreLocationVersion { get; private set; }

  public static ConfigEntry<bool> SetCustomFieldsForAshlandsZDOs { get; private set; }

  public static ConfigFile BindConfig(ConfigFile config) {
    IgnoreGenerateLocationsIfNeeded =
        config.BindInOrder(
            "ZoneSystem",
            "ignoreGenerateLocationsIfNeeded",
            false,
            "If set, ignores any calls to ZoneSystem.GenerateLocationsIfNeeded().");

    IgnoreLocationVersion =
        config.BindInOrder(
            "ZoneSystem",
            "ignoreLocationVersion",
            false,
            "If set, ignores the ZoneSystem.m_locationVersion check in ZoneSystem.Load().");

    SetCustomFieldsForAshlandsZDOs =
        config.BindInOrder(
            "ZDOMan",
            "setCustomFieldsForAshlandsZDOs",
            false,
            "If set, will set custom fields for specific ZDOs in Ashlands biome.");

    return config;
  }
}
