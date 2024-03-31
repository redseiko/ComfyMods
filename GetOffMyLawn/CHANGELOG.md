## Changelog

### 1.7.0

  * Updated for the `v0.217.38` patch.
  * Moved changelog into `CHANGELOG.md`.
  * Bumped up `<LangVersion>` to C# 10.
  * Removed custom config drawer as the functionality is now in `Configula` mod.
  * Modified Piece repair logic to work with vanilla `WearNTear` visual updates and claim ZDO ownerhsip on repair.
  * Minor code clean-up and refactoring.

### 1.6.0

  * Updated for `v0.216.5` PTB patch.

### 1.5.0

  * Modified TargetPieceHealth config setting to use FloatConfigEntry with custom text-to-float parsing/validation.
  * Removed the ApplyDamageCount logging since it's no longer needed info.
  * Minor code clean up.

### 1.4.1

  * Repairs with negative damage should now take effect. Vanilla repair previously overwrote negative repair values.

### 1.4.0

  * Moved all configuration code into new `PluginConfig` class.
  * Moved all Harmony-patching code into their own patch classes.
  * **Increased the default `PieceHealth` value to `1E+17`.**
  * Added `manifest.json` and updated this `README.md`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.3.1

  * Destroy carts & boats with the Hammer like regular build pieces.

### 1.2.1

  * Actually check the `enablePieceHealthDamageThreshold` config value for the WearNTear.ApplyDamage() patch.

### 1.2.0

  * Added new optimization/configuration option `enablePieceHealthDamageThreshold`.
    * Pieces with health that exceed 100K **will not** execute `WearNTear.ApplyDamage()` meaning they will not
      take any piece damage. Subsequently, they **will not** send a `WNTHealthChanged` message to the server.
    * This reduces the overall send and receive rates for every player on the server as they will no longer receive
      the message used only for syncing the visual condition of pieces across clients.

### 1.0.1

  * Added null-checks for Piece and Piece.ZNetView references in the ward-interaction method.

### 1.0.0

  * Updated for Hearth & Home.