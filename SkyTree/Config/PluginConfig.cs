namespace SkyTree;

using BepInEx.Configuration;

using ComfyLib;

public class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    IsModEnabled.OnSettingChanged(YggdrasilManager.ToggleYggdrasil);
  }
}
