namespace Unbuildable;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigFile CurrentConfig { get; private set; }

  public static ConfigEntry<string> PrefabsToDestroyList { get; private set; }

  public static void BindConfig(ConfigFile config) {
    CurrentConfig = config;

    PrefabsToDestroyList =
        config.BindInOrder(
            "Prefabs",
            "prefabsToDestroyList",
            string.Empty,
            "Comma-separated list of prefabs to destroy on placement.");

    PrefabsToDestroyList.OnSettingChanged(OnPrefabsToDestroyListChanged);
    OnPrefabsToDestroyListChanged();
  }

  static void OnPrefabsToDestroyListChanged() {
    ZDOManUtils.SetPrefabsToDestroy(PrefabsToDestroyList.GetStringValues());
  }
}
