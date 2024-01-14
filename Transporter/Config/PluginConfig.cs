using BepInEx.Configuration;

namespace Transporter {
  public static class PluginConfig {
    public static ConfigEntry<string> AccessListFilename { get; private set; }
    public static ConfigEntry<string> AccessLogFilename { get; private set; }

    public static void BindConfig(ConfigFile config) {
      AccessListFilename =
          config.Bind(
              "Access",
              "accessListFilename",
              "transporter-access-list.txt",
              "SyncedList filename containing SteamIds that have access to Transporter.");

      AccessLogFilename =
          config.Bind(
              "Access",
              "accessLogFilename",
              "transport-access-log.txt",
              "Textfile filename for logging all access to Transporter.");
    }
  }
}
