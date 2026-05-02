namespace BetterServerPortals;

using BepInEx.Configuration;

public static class PluginConfig {
  public static ConfigEntry<float> ConnectPortalCoroutineWait { get; private set; }

  public static ConfigEntry<string> AdditionalPortalPrefabs { get; private set; }

  public static ConfigEntry<string> RandomPortalsAccessListFilename { get; private set; }

  public static void BindConfig(ConfigFile config) {
    ConnectPortalCoroutineWait =
        config.Bind(
            "Portals",
            "connectPortalCoroutineWait",
            5f,
            "Wait time (seconds) when ConnectPortal coroutine yields.");

    AdditionalPortalPrefabs =
        config.Bind(
            "Portals",
            "additionalPortalPrefabs",
            string.Empty,
            "Comma-separated list of prefab-names that will be added to Game.PortalPrefabHash list.");

    RandomPortalsAccessListFilename =
        config.Bind(
            "RandomPortals",
            "accessListFilename",
            "bsp-random-portals-access-list.txt",
            "Filename for SyncedAuthList containing SteamIds allowed to set RandomPortal tags.");
  }
}
