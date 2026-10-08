namespace Perpetuity;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<bool> IsPersistentEventSystemEnabled { get; private set; }
  public static ConfigEntry<bool> ApplyPersistentEventSystemOverrides { get; private set; }
  public static ConfigEntry<bool> CenterPersistentEventPosition { get; private set; }
  public static ConfigEntry<bool> LocationInstancesBlockPeristentEventPlacement { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsPersistentEventSystemEnabled =
        config.BindInOrder(
            "PersistentEventSystem",
            "isEnabled",
            true,
            "If false, active PersistentEvents are cleared and incoming RequestStartEvent RPCs are ignored.");

    ApplyPersistentEventSystemOverrides =
        config.BindInOrder(
            "PersistentEventSystem",
            "applyOverrides",
            true,
            "If true, applies overrides to PersistentEvents.");

    CenterPersistentEventPosition =
        config.BindInOrder(
            "PersistentEvent",
            "centerPosition",
            true,
            "If true, PersistentEvent position will be overriden to the *center* of the sector.");

    LocationInstancesBlockPeristentEventPlacement =
        config.BindInOrder(
            "PersistentEvent",
            "locationInstancesBlockPlacement",
            true,
            "If true, PersistentEvents will not spawn in sectors with an existing LocationInstance.");
  }
}
