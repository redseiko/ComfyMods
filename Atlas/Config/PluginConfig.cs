namespace Atlas;

using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

public static class PluginConfig {
  public static ConfigEntry<bool> IgnoreGenerateLocationsIfNeeded { get; private set; }
  public static ConfigEntry<bool> IgnoreLocationVersion { get; private set; }

  public static ConfigEntry<bool> OverrideServerPosition { get; private set; }
  public static ConfigEntry<Vector3> CustomServerPosition { get; private set; }

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

    OverrideServerPosition =
        config.BindInOrder(
            "Game",
            "overrideServerPosition",
            true,
            "If set, overrides the value for SetReferencePosition() called in Game.FixedUpdate().");

    CustomServerPosition =
        config.BindInOrder(
            "Game",
            "customServerPosition",
            new Vector3(12800f, 0f, 12800f),
            "Custom value to use for server position when overrideServerPosition is set.");

    return config;
  }
}
