namespace Parrot;

using BepInEx.Configuration;

public static class PluginConfig {
  public static ConfigEntry<bool> AddServerToPlayerList { get; private set; }
  public static ConfigEntry<bool> AllowParrotServerConnections { get; private set; }

  public static void BindConfig(ConfigFile config) {
    AddServerToPlayerList =
        config.Bind(
            "PlayerList",
            "addServerToPlayerList",
            true,
            "If set, will add the current server to the ZNet.PlayerInfo list sent in ZNet.SendPlayerList().");

    AllowParrotServerConnections =
        config.Bind(
            "ParrotServer",
            "allowParrotServerConnections",
            false,
            "Allows incoming Parrot-server connections (experimental).");
  }
}
