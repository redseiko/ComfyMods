## Changelog

### 1.11.0

  * Converted all commands to `ComfyCommand` and `ComfyArgs` format.
  * Renamed commands:
    * `startkeymanager` -> `start-key-manager`
    * `stopkeymanager` -> `stop-key-manager`
  * Removed `startvendorkeymanager` command and merged it into `start-key-manager` command.

### 1.10.0

  * Updated for the `v0.221.4` patch.
  * Added `ComfyArgs` support for `ComfyCommand`.
  * Converted logic from a `CachedStringList` to `CachedStringSet`.
  * Added `GlobalKeysManager.RemovePeer()` invoked from `ZRoutedRpc.RemovePeer()` to update all `NearbyPeerIds`.
  * Code clean-up and refactoring.
  * Updated mod icon.

### 1.9.0

  * Fixed for the `v0.220.3` patch.
  * Bumped up `<LangVersion>` to C# 12.
  * Code clean-up and refactoring.

### 1.8.0

  * Moved changelog into `CHANGELOG.md`.
  * Bumped up `<LangVersion>` to C# 10.

### 1.7.0

  * Updated for `v0.217.38` patch.
  * Converted commands to `ComfyCommand` format.
  * Add support for loading/saving KeyManager commands to run at start-up.

### 1.6.0

  * Updated for `v0.217.28` PTB patch.
  * Updated project references from `unstripped_corlib` to `valheim_server_Data\Managed`.

### 1.5.0

  * Fixed for the `v0.217.22` patch.
  * Added new `GetEncodedGlobalKeys()` extension to enable specifying global keys with spaces by using `=` instead.
  * Added in temporary-fix to ensure `StopKeyManager` command actually restores original global keys.

### 1.4.0

  * Add new `GlobalKeysManager` and several new commands related to KeyManager starting and stopping.

### 1.3.0

  * Updated for `v0.217.14` patch.
  * Modified `ZoneSystem.Load()` patch to modify the globalkeys list with `globalKeysAllowedList` if specified.

### 1.2.0

  * Updated for `v0.216.8` PTB patch.
  * Modified `.csproj` to reference Valheim dedicated server DLLs.

### 1.1.1

  * Updated for `v0.216.5` PTB patch.

### 1.1.0

  * Fixed for the `v0.214.2` patch.
  * Added more egg puns for Haldor to say.

### 1.0.0

  * Initial release.
