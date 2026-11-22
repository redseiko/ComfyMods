namespace Eventuality;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<string> AccessListFilename { get; private set; }
  public static ConfigEntry<string> AccessLogFilename { get; private set; }

  public static ConfigEntry<string> RandomEventLogFilename { get; private set; }

  public static ConfigEntry<Heightmap.Biome> RandomEventBiomesToExclude { get; private set; }

  public enum PlayerBaseRequirement {
    Default,
    NearBase,
    AwayFromBase,
    Anywhere,
  }

  public static ConfigEntry<PlayerBaseRequirement> CheckPlayerBaseRequirement { get; private set; }
  public static ConfigEntry<int> MinimumPlayerBaseValue { get; private set; }

  public enum PauseEventRequirement {
    Default,
    PauseIfNoPlayer,
    NeverPause,
  }

  public static ConfigEntry<PauseEventRequirement> PauseIfNoPlayerRequirement { get; private set; }

  public static void BindConfig(ConfigFile config) {
    AccessListFilename =
        config.BindInOrder(
            "Access",
            "accessListFilename",
            "eventuality-access-list.txt",
            "Filename for text-file containing SteamIds allowed to start/end RandomEvents on the server.");

    AccessLogFilename =
        config.BindInOrder(
            "Access",
            "accessLogFilename",
            "eventuality-access-log.txt",
            "Filename for csv-file to write RPC_SetEvent access logging.");

    RandomEventLogFilename =
        config.BindInOrder(
            "Logging",
            "randomEventLogFilename",
            "eventuality-random-event-log.txt",
            "Filename for csv-file to write RandEventSystem event start/end logging.");

    RandomEventBiomesToExclude =
        config.BindInOrder(
            "RandomEvent",
            "biomesToExclude",
            Heightmap.Biome.None,
            "Biomes to exclude when checking for valid player positions in RandEventSystem.InValidBiome().");

    CheckPlayerBaseRequirement =
        config.BindInOrder(
            "RandomEvent",
            "checkPlayerBaseRequirement",
            PlayerBaseRequirement.Default,
            "Player base requirement mode when checking for valid player positions in RandEventSystem.CheckBase().");

    MinimumPlayerBaseValue =
        config.BindInOrder(
            "RandomEvent",
            "minimumPlayerBaseValue",
            3,
            "Minimum player base value required to be considered near a base in RandEventSystem.CheckBase().",
            new AcceptableValueRange<int>(0, 100));

    PauseIfNoPlayerRequirement =
        config.BindInOrder(
            "RandomEvent",
            "pauseIfNoPlayerRequirement",
            PauseEventRequirement.Default,
            "Pause requirement mode for RandomEvent.m_pauseIfNoPlayerInArea on event start.");
  }
}
