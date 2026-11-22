# Eventuality

*Server-side RandEventSystem customization and control.*

## Commands

### start-random-event

    start-random-event
       --event=<name>
      [--position=<x,y,z>]     [--pos=<x,y,z>]

  * Starts a `RandomEvent` on the server
  * If there is already an existing `RandomEvent` that one will be stopped first.
  * `--event=<name>`
    * Starts the `RandomEvent` with matching `<name>` (if eligible).
    * If `random` is specified for `--event`, selects and starts a random eligible `RandomEvent`.
  * `--position=<x,y,z>`
    * If `--position` is not specified, will be set to a random valid player position (if possible).
    * If `--position` is not valid for specified `RandomEvent` will return an error.
    * `y` value must be less than `3000`.

### stop-random-event

    stop-random-event

  * Stops any current `RandomEvent` on the server.

## Configuration

`[Access]`

  * `accessListFilename`
    * Filename for text-file containing SteamIds allowed to start/end `RandomEvents` on the server.
    * Default: `eventuality-access-list.txt`
  * `accessLogFilename`
    * Filename for csv-file to write `RPC_SetEvent` access logging.
    * Default: `eventuality-access-log.txt`

`[Logging]`

  * `randomEventLogFilename`
    * Filename for csv-file to write `RandEventSystem` event start/end logging.
    * Default: `eventuality-random-event-log.txt`

`[RandomEvent]`

  * `biomesToExclude`
    * Biomes to exclude when checking for valid player positions in `RandEventSystem.InValidBiome()`.
    * Options:
      * `None`, `All`
      * `Meadows`, `BlackForest`
      * `Swamp`, `Mountain`, `Plains`
      * `AshLands`, `Mistlands`, `DeepNorth`
      * `Ocean`, `Land`
    * Default: `None`
    * Multiple options can be set, separated using commas `,`.
  * `checkPlayerBaseRequirement`
    * Player base requirement mode when checking for valid player positions in `RandEventSystem.CheckBase()`.
    * Options: `Default`, `NearBase`, `AwayFromBase`, `Anywhere`
    * Default: `Default`
  * `minimumPlayerBaseValue`
    * Minimum player base value required to be considered near a base in `RandEventSystem.CheckBase()`.
    * Default: `3`
  * `pauseIfNoPlayerRequirement`
    * Pause requirement mode for `RandomEvent.m_pauseIfNoPlayerInArea` on event start.
    * Options: `Default`, `PauseIfNoPlayer`, `NeverPause`
    * Default: `Default`

## Notes

  * See source at: [GitHub/ComfyMods](https://github.com/redseiko/ComfyMods/tree/main/Eventuality).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
