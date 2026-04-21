namespace ConstructionDerby;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }
  public static ConfigEntry<bool> TestingCanRemovePiece { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    IsModEnabled.OnSettingChanged(ComfyCommandUtils.ToggleCommands);

    TestingCanRemovePiece =
        config.BindInOrder(
            "Testing",
            "canRemovePiece",
            false,
            "Control if Player can remove a placed piece.");
  }
}
