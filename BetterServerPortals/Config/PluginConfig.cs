namespace BetterServerPortals;

using BepInEx.Configuration;

public static class PluginConfig {
  public static ConfigEntry<float> ConnectPortalCoroutineWait { get; private set; }

  public static ConfigEntry<string> RandomPortalsAccessListFilename { get; private set; }

  public static void BindConfig(ConfigFile config) {
    ConnectPortalCoroutineWait =
        config.Bind(
            "Portals",
            "connectPortalCoroutineWait",
            5f,
            "Wait time (seconds) when ConnectPortal coroutine yields.");

    RandomPortalsAccessListFilename =
        config.Bind(
            "RandomPortals",
            "accessListFilename",
            "bsp-random-portals-access-list.txt",
            "Filename for SyncedList containing SteamIds allowed to set RandomPortal tags.");
  }
}
