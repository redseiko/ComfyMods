## Changelog

### 1.8.0

  * Fixed for the `v0.220.3` patch.
  * Bumped up `<LangVersion>` to C# 12.
  * Code clean-up and refactoring.
  * `SayHandler` now checks if `targetPeerId` is equal to `0L` or `serverPeerId` to execute.

### 1.7.0

  * Updated for the `v0.218.12` PTB patch and `BetterZeeRouter v1.9.0` update.
  * Moved changelog into `CHANGELOG.md`.
  * Bumped up `<LangVersion>` to C# 10.
  * Code clean-up and refactoring.

### 1.6.0

  * Updated for `v0.217.28` patch.
  * Updated project references from `unstripped_corlib` to `valheim_server_Data\Managed`.

### 1.5.0

  * Updated for `v0.216.8` PTB patch.
  * Simplified `SendDiceRollResponse()` to use the vanilla `InvokeRoutedRPC()` behaviour.
  * Modified `.csproj` to reference Valheim dedicated server DLLs.
  * Added `manifest.json`, `icon.png` and modified the `.csproj` to create a versioned Thunderstore package.

### 1.4.0

  * Updated for `v0.214.2` patch.

### 1.3.0

  * Create this `README.md` and backfill it with commit messages.
  * Fix dice result response broken by in `v0.211.7` patch due to `RPC_Say` adding an extra argument.

### 1.2.0

  * Modified CriticalDice to be dependent on BetterZeeRouter as it can no longer use a transpiler..
  * Modified the SayHandler to extract the necessary params and pass them forward as arguments.

### 1.1.1

  * Fixed a bug in CriticalDice regex for simple number so that having a trailing space is optional.

### 1.1.0

  * Rewrote the entire mod to run within an Coroutine and async Task to offload from the main network thread.
  * Bypass calling ZRoutedRpc.RouteRPC() and create the package data directly and queue it onto each RPC.  

### 1.0.0

  * Initial release.