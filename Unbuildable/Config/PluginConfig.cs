namespace Unbuildable;

using System;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigFile CurrentConfig { get; private set; }

  public static ConfigEntry<string> PrefabsToRemoveList { get; private set; }
  public static ConfigEntry<string> PrefabsToDestroyList { get; private set; }

  public static void BindConfig(ConfigFile config) {
    CurrentConfig = config;

    PrefabsToRemoveList =
        config.BindInOrder(
            "Prefabs",
            "prefabsToRemoveList",
            string.Empty,
            "Comma-separated list of prefabs (with WearNTear) to remove via RPC_Remove on ZDO creation.");

    PrefabsToRemoveList.OnSettingChanged(OnPrefabsToRemoveListChanged);
    OnPrefabsToRemoveListChanged();

    PrefabsToDestroyList =
        config.BindInOrder(
            "Prefabs",
            "prefabsToDestroyList",
            string.Empty,
            "Comma-separated list of prefabs to destroy directly on ZDO creation.");

    PrefabsToDestroyList.OnSettingChanged(OnPrefabsToDestroyListChanged);
    OnPrefabsToDestroyListChanged();
  }

  static void OnPrefabsToRemoveListChanged() {
    ZDOManUtils.SetPrefabsToRemove(PrefabsToRemoveList.Value.AsSpan().GetStableHashCodes());
  }

  static void OnPrefabsToDestroyListChanged() {
    ZDOManUtils.SetPrefabsToDestroy(PrefabsToDestroyList.Value.AsSpan().GetStableHashCodes());
  }
}
