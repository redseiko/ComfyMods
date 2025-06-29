## Changelog

### 1.4.0

  * Updated for the `v0.220.5` patch.
  * Bumped up `<LangVersion>` to C# 12.
  * Code clean-up and refactoring.
  * Added new config-options under `[EnemyHud.HealthText]`:
    * `showMaxHealth` -- show max health value.
    * `showInfiniteHealth` -- show infinite symbol when current health exceeds threshold.
    * `infiniteHealthThreshold` -- threshold to show the infinite symbol for current health.

### 1.3.2

  * Fixed `EnemyHud` not positioned correctly when using Render-scale values not at 100%.

### 1.3.1

  * Actually include the `CHANGELOG.md` in the Thunderstore archive.

### 1.3.0

  * Updated for the `v0.217.38` patch.
  * Moved changelog into `CHANGELOG.md`.
  * Added new config option `enemyLevelUseVanillaStar` defaulting to false.
  * Changed symbols for config option `enemyLevelStarSymbol` as some previous symbols were no longer available.
  * Re-added gradient effect for boss names, config option `BossHud.Name.nameTextColor` changed to two separate colors.

### 1.2.1

  * Fixed a bug for EnemyHud names were being word-wrapped.

### 1.2.0

  * Fixed for the `v0.217.24` patch.
  * Migrated all `UI.Text` components to `TextMeshPro`.
  * Had to remove the cool `VerticalGradient` effect as it's not compatible with `TextMeshPro`.

### 1.1.0

  * Updated for the `v0.214.2` patch.
  * Updated BepInEx dependency to `denikson-BepInExPack_Valheim-5.4.2100`.
  * Removed `BossHud.Name.useGradientEffect` config option for now as it is not compatible with TextMeshPro.

### 1.0.0

  * Initial release.
