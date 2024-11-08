namespace ComfyAutoRepair;

using BepInEx.Configuration;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<bool> ShowVanillaRepairMessage { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.Bind(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    ShowVanillaRepairMessage =
        config.Bind(
            "Message",
            "showVanillaRepairMessage",
            false,
            "If set, shows the vanilla repair message for each item repaired.");
  }
}
