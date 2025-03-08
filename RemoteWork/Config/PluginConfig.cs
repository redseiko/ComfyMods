namespace RemoteWork;

using BepInEx.Configuration;

public static class PluginConfig {
  public static ConfigEntry<string> AccessListFilename { get; private set; }
  public static ConfigEntry<string> AccessLogFilename { get; private set; }

  public static void BindConfig(ConfigFile config) {
    AccessListFilename =
        config.Bind(
            "RemoteWork.Access",
            "accessListFilename",
            "remote-work-access-list.txt",
            "Filename for SyncedList that contains SteamIds for access to RemoteWork.");

    AccessLogFilename =
        config.Bind(
            "RemoteWork.Access",
            "accessLogFilename",
            "remote-work-access-log.txt",
            "Filename for textfile that will log all access/commands to RemoteWork.");
  }
}
