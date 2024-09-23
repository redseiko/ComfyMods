namespace BetterServerPortals;

using BepInEx.Configuration;

public static class PluginConfig {
  public static ConfigEntry<float> ConnectPortalCoroutineWait { get; private set; }

  public static void BindConfig(ConfigFile config) {
    ConnectPortalCoroutineWait =
        config.Bind(
            "Portals",
            "connectPortalCoroutineWait",
            5f,
            "Wait time (seconds) when ConnectPortal coroutine yields.");
  }
}
