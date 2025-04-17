namespace FabulousSteam;

using BepInEx.Configuration;

public static class PluginConfig {
  public static ConfigFile CurrentConfig { get; private set; }

  public static void BindConfig(ConfigFile config) {
    CurrentConfig = config;
  }
}
