## Changelog

### 1.13.0

  * Fixed for the `v0.220.4` patch.
  * Simplified `PlatformUserID.Constructor()` transpiler patch due to changes in this update.

### 1.12.0

  * Updated for the `v0.220.3` patch.
  * Added a `PlatformUserID.Constructor()` transpiler patch to filter out all the failed parsing log spam.
  * Copied the `Container.RPC_RequestOpen()` transpiler patches for `RPC_RequestStack()` and `RPC_RequestTakeAll()`.

### 1.11.0

  * Updated for the `v0.219.10` PTB patch.
  * Minor optimization to `ZLog.Log()` patch.

### 1.10.1

  * Fixed a bug `Container.RPC_RequestOpen()` transpiler patch.

### 1.10.0

  * Updated for the `v0.218.19` patch.
  * Added a `Container.RPC_RequestOpen()` transpiler patch to filter out "Player wants to open" log spam.

### 1.9.0

  * Updated for the `v0.218.15` Ashlands patch.
  * Added a new `UIGroupHandler.Update()` transpiler patch to filter out "Activating ... element" log spam.
  * Rewrote existing `ZSteamSocket.SendQueuedPackages()` transpiler to use branching instead of overwrites.

### 1.8.0

  * Added a `Projectile.FixedUpdate()` transpiler patch to handle zero `m_vel` passed to `Quaternion.LookRotation()`.
    * This should reduce the amount of "Look rotation viewing vector is zero" Unity log spam.
    * Can be toggled using the `checkProjectFixedUpdatedZeroVelocity` config option (restart required).

### 1.7.0

  * Updated for the `v0.217.38` patch.
  * Moved changelog into `CHANGELOG.md`.
  * Bumped up `LangVersion` to C# 10.

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
