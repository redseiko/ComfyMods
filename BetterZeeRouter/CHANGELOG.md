## Changelog

### 1.7.0

  * Moved changelog into `CHANGELOG.md`.
  * Added new config options for the `SetTargetHandler`: `shouldCheckDistance` and `distanceCheckRange`.

### 1.6.0

  * Updated for `v0.217.28` patch.
  * Updated project references from `unstripped_corlib` to `valheim_server_Data\Managed`.

### 1.5.0

  * Fixed for `v0.216.8` PTB patch.
  * Modified .csproj to now target Valheim dedicated server DLLs.
  * Removed the conditional `isModEnabled` config to simplify logic.
  * Renamed `TeleportToHandler` to `TeleportPlayerHandler` and added a `TeleportPlayerAccess.txt` SyncedList.
  * Moved `ZRoutedRpc` patch logic into its own class and cleaned up some references.

### 1.4.0

  * Extracted config logic into a separate class.
  * Extracted RPC hashcodes into a separate class.
  * Moved extension files into their own folder.
  * Removed the WNTHealthChanged/DamageText logging as it's no longer really needed.
  * Added `manifest.json`, `icon.png` and modified this `README.md`.
  * Modified the project file to create a versioned Thunderstore package.

### 1.3.0

  * Extract all existing RpcMethodHandlers into separate classes.
  * Create `SetTargetHandler` for the Mistland's Turret `RPC_SetTarget` call.

### 1.2.0

  * ???