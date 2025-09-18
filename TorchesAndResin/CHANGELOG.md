## Changelog

### 1.8.0

  * Added logic for eligible over-fueled candles when updating state to never be considered wet or underwater.
  * Added new config-option to control new feature:
    * `[Candle] candleUpdateStateNeverWet`

### 1.7.0

  * Added new hover-text for candles to show their fuel-value.
  * Added logic to toggle-on candles when setting their fuel-value.
  * Added new config-options to control the new features above:
    * `[Candle] candleHoverTextShowFuel`
    * `[Candle] candleAlwaysToggleOn`
  * Updated mod icon.

### 1.6.0

  * Updated for the `v0.220.5` patch.
  * Code clean-up and refactoring for improved performance.
  * `Candle_resin` added to list of eligible torches.

### 1.5.0

  * Updated for `v0.217.37` PTB.
  * Fixed a bug where mod was using PluginVersion instead of PluginGuid for HarmonyId.
  * Simplified `Fireplace` patch logic.
  * Added new config option `torchStartingFuel` as a range value, defaulting to a minimum value of `10000`.

### 1.4.0

  * Added `fire_pit_iron` to the supported list.
  * Extracted patch logic and config logic into separate classes.

### 1.3.0

  * Added a `Fireplace.Awake` prefix patch that sets an eligible torch's `m_startFuel` to 10000.
  * Added `manifest.json`, changed the `icon.png` and updated this `README.md`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.2.0

  * Updated for Hearth & Home.
  * Added a Transpiler to the Fireplace.Awake() method to change the initial UpdateFireplace.Invoke() from 0s to 0.5s.

### 1.1.0

  * Added braziers to list of torches supported.
  * Use `ZNetView.m_prefabName` instead of `GameObject.name`.

### 1.0.0

  * Updated project template and references to use latest DLLs.
