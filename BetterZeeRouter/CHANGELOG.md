## Changelog

### 1.10.0

  * Fixed for the `v0.220.3` patch.
  * Bumped up `<LangVersion>` to C# 12.
  * Code clean-up and refactoring.
  * `RoutedRpcManager.ProcessRoutedRPC()` will now also perform RPC routing if `targetPeerId == serverPeerId`.

### 1.9.0

  * Fixed for the `v0.218.12` PTB patch.
  * Converted `RoutedRpcManager` to static singleton and modified its `AddHandler()` interface.
  * Renamed `WntHealthChangedHandler` to `HealthChangedHandler` as `WearNTear` RPC method names have changed.
  * Code clean-up and refactoring.

### 1.8.0

 * Updated for the `v0.217.46` patch.
 * Bumped up `<LangVersion>` to C# 10.

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