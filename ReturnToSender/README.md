# ReturnToSender

*Revert ZDOMan.Update to use the original SendZDOToPeers method.*

## Installation

### Manual

  * Un-zip `ReturnToSender.dll` to your `/Valheim/BepInEx/plugins/` folder.

### Thunderstore (manual)

  * Go to Settings > Import local mod > Select `ReturnToSender_v1.2.0.zip`.
  * Click "OK/Import local mod" on the pop-up for information.

## Changelog

### 1.2.0

  * Fixed for `v0.217.28` PTB patch.
  * Updated project references from `unstripped_corlib` to `valheim_server_Data\Managed`.

### 1.1.0

  * Fixed for `v0.216.8` PTB patch.
  * Re-implemented `SendZDOToPeers()` method since it was removed in vanilla (RIP).

### 1.0.0

  * Initial release.