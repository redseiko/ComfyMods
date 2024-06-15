namespace Unbuildable;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigFile CurrentConfig { get; private set; }
  public static ConfigEntry<bool> IsModEnabled { get; private set; }
  public static ConfigEntry<string> PrefabsToDestroyList { get; private set; }

  public static void BindConfig(ConfigFile config) {
    CurrentConfig = config;

    IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

    PrefabsToDestroyList =
        config.BindInOrder(
            "Prefabs",
            "prefabsToDestroyList",
            string.Empty,
            "Comma-separated list of prefabs to destroy on placement.");
  }
}
