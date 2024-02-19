namespace Transporter;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<string> AccessListFilename { get; private set; }
  public static ConfigEntry<string> AccessLogFilename { get; private set; }

  public static ConfigEntry<string> RequestListFilename { get; private set; }

  public static void BindConfig(ConfigFile config) {
    AccessListFilename =
        config.BindInOrder(
            "Access",
            "accessListFilename",
            "transporter-access-list.txt",
            "SyncedList filename containing SteamIds that have access to Transporter.");

    AccessLogFilename =
        config.BindInOrder(
            "Access",
            "accessLogFilename",
            "transporter-access-log.txt",
            "Textfile filename for logging all access to Transporter.");

    RequestListFilename =
        config.BindInOrder(
            "Request",
            "requestListFilename",
            "transporter-request-list.txt",
            "SyncedList filename containing pending teleport requests for Transporter.");
  }
}
