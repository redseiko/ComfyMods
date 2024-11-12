namespace YellowPages;

using BepInEx.Configuration;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.Bind(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");
  }
}
