namespace PotteryBarn;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod (restart required).");
  }
}
