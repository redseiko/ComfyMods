namespace FabulousSteam;

using BepInEx.Configuration;

public static class PluginConfig {
  public static ConfigFile CurrentConfig { get; private set; }

  public static ConfigEntry<int> PlayFabServerPort { get; private set; }

  public static void BindConfig(ConfigFile config) {
    CurrentConfig = config;

    PlayFabServerPort =
        config.Bind(
            "PlayFab",
            "playFabServerPort",
            -1,
            "If > 0, will override the port the PlayFab server should listen to.");
  }
}
