# Atlas

*Shrugged?*

## Changelog

### 1.9.0

  * Updated for `v0.217.28` PTB patch.
  * Updated project references from `unstripped_corlib` to `valheim_server_Data\Managed`.
  * Fixed the `ZDOMan.Load()` transpiler to properly emit an unconditional branch.

### 1.8.0

  * Fixed for the `v0.217.22` patch.

### 1.7.0

  * Re-added console command `setworldtime` with adjustment for custom `timeCreated` ZDO long property.

### 1.6.0

  * Added feature where server will cache/embed a ZDO's UID into its data for reference after a server restart.
  * Added a hacky feature for maintaining/appending server SessionIds in a world file using custom metadata ZDOs.

### 1.5.7

  * Remove `ZDO.Deserialize` prefix-patch temporarily to help debug prefab duplication issues.

### 1.5.6

  * Fixed some edge-cases for handling duplicate ZDOs.

### 1.5.5

  * Added `ZDO.Deserialize` prefix-patch that rewrites entire method to enable removing ZDOExtraData key/values.

### 1.5.4

  * Added logic to stamp ZDOs with `epochTimeCreated` in addition to `timeCreated`.
  * Added `PluginLogger` and changed all logging references to use it.
  * Added `manifest.json` and `icon.png`, modified `.csproj` to generate a versioned Thunderstore package.

### 1.5.3

  * Removed the entire `ZDOMan.Save()` async-related code (and problems) as vanilla cloning is fast enough.

### 1.5.2

  * Modified the `ZDOMan.Load()` transpiler-patch to fix an edge-case when loading a world prior to PTB.

### 1.5.1

  * Added a `ZDOMan.ConnectSpawners()` prefix-patch that rewrites the method to run in `O(n)` time.

### 1.5.0

  * Fixed for `v0.216.7` PTB patch.
  * Removed `ZDOMan.SaveAsync()` patch as the ZPackage-BinaryWriter optimization is now in vanilla code.
  * Removed `setworldtime` console command as `ZDO.m_timeCreated` is removed and no longer set by clients.
  * Removed `ZoneSystem.ignorePgwVersion` config and related code as field no longer exists.
  * Re-implemented `ZDO.m_timeCreated` in a not-so-efficient manner.

### 1.4.0

  * Created `PluginConfig` and added several ZoneSystem-related config options.
  * Added a new Terminal.ConsoleCommand `setworldtime <time>`.
  * Added a `ZNet.LoadWorld` postfix-patch to log the m_netTime read from the world file.

### 1.3.0

  * Extract various patch code into separate classes.
  * Rewrite `ZDOMan.SaveAsync()` to bypass an un-needed byte array copy, add more accurate timing.

### 1.2.0

  * Created this `README.md`.
  * Added temporary ZoneSystem patch to bypass checks that result in locationInstances to be cleared and regenerated.

### 1.1.0

  * Rewrite `ZDOMan.Load()` to handle possible duplicate ZDOs (same ZDOID) that can occur with async saving.

### 1.0.0

  * Initial release.