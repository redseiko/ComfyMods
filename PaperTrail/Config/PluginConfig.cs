namespace PaperTrail;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigFile CurrentConfig { get; private set; }
  public static ConfigEntry<string> PickablePrefabsToLogList { get; private set; }

  public static void BindConfig(ConfigFile config) {
    CurrentConfig = config;

    PickablePrefabsToLogList =
        config.BindInOrder(
            "Pickable",
            "pickablePrefabsToLogList",
            "goblin_totempole",
            "Comma-separated list of Pickable prefabs to log RPCs for.");

    PickablePrefabsToLogList.OnSettingChanged(PickablePrefabsToLogListChanged);
    PickablePrefabsToLogListChanged();
  }

  static void PickablePrefabsToLogListChanged() {
    PickableManager.SetPickablePrefabsToLog(PickablePrefabsToLogList.GetStringValues());
  }
}
