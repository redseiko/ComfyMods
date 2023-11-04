# BetterZeeLog

*Logging zees the better way.*

## Features

  * Modifies all `ZLog.Log*` methods to prefix a timestamp and remove the trailing new-line.
  * Removes stack traces for `Info` and `Warning` log types.
  * Removes the 'Failed to send data' logging in `ZSteamSocket` to reduce log spam.

## Notes

  * See source at: [GitHub/ComfyMods](https://github.com/redseiko/ComfyMods/tree/main/BetterZeeLog).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
  * BetterZeeLog icon created by [@jenniely](https://twitter.com/jenniely) (jenniely.com)

## Changelog

### 1.6.0

  * Fixed for `v0.217.28` PTB patch.
  * Updated project references from `unstripped_corlib` to `valheim_server_Data\Managed`.

### 1.5.0

  * Fixed for the `v0.217.22` patch.
  * Changed all `ZLog` prefix patches to instead be transpiler patches to fix a bug when used with dedicated servers.
  * Added a dumb `ZLog.Log` prefix patch to catch messages starting with "`Console: `" to handle an edge-case with
    dedicated servers calling `ZLog.Log()` in `Terminal.AddString(string)`.
    * Transpiler patching `Terminal.AddString(string)` fails for no apparent reason hence this dumb patch.

### 1.4.2

  * Updated mod icon to a new one created by [@jenniely](https://twitter.com/jenniely).

### 1.4.1

  * Fixed the PluginVersion not being referenced in `AssemblyInfo.cs`.
  * Expanded this `README.md`.

### 1.4.0

  * Updated for Valheim `v0.212.6` Mistlands PTB.
  * Extracted configuration logic into `PluginConfig` class.
  * Extracted patch logic into separate patch classes.
  * Added configuration option `RemoveFailedToSendDataLogging` for the ZSteamSocket transpiler patch.

### 1.3.0

  * Updated for Valheim `v0.209.5`.
  * Fixed the `ZSteamSocket.SendQueuedPackages` transpiler.
  * Added `manifest.json`, `icon.png` and `README.md`.
  * Modified the project file to create a versioned Thunderstore package.