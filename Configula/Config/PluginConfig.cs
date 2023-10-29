using BepInEx.Configuration;

using ComfyLib;

namespace Configula {
  public static class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }

    public static ConfigEntry<int> WindowWidth { get; private set; }

    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      WindowWidth =
          config.BindInOrder(
              "Window",
              "windowWidth",
              650,
              "Width of the ConfigurationManager window.",
              new AcceptableValueRange<int>(0, 1920));
    }
  }
}
