# TorchesAndResin

*Standing wood/iron/wall torches and hanging/floor braziers are refueled with 10,000 resin.*

## Instructions

  * This will set the fuel for several torch and brazier prefabs to 10,000 resin.
  * Fuel will be set when prefab is placed or on entering the sector as the sole instance owner.

### Notes

  * See source at: [GitHub](https://github.com/redseiko/ComfyMods/tree/main/TorchesAndResin).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)

## Changelog

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