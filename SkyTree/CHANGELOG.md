## Changelog

### 1.6.0

  * Updated for the `v0.221.12` patch.
  * Completely refactored to use `EnvMan` for patch logic.
  * Mod can now be toggled on/off while in-game.
  * Added new `ZoneSystem.IsBlocked()` patch to try and skip `Yggdrasil` collider when raycasting.
  * Updated mod icon.

### 1.5.0

  * Added new `ZoneSystem.SpawnZone()` patches to change Yggdrasil's layer to skybox during zone generation.

### 1.4.0

  * Updated for `v0.216.9` patch.
  * Properly uses the PluginGuid for the HarmonyInstanceId instead of the PluginVersion.

### 1.3.0

  * Moved all configuration code into new `PluginConfig` class.
  * Added some more logging when changing layers to get the layer name.
  * Added `manifest.json`, changed the `icon.png` and updated this `README.md`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.2.0

  * Updated for Hearth & Home.
  * Fixed a bug in adding a collider to the SkyTree.

### 1.1.0

  * Updated project template and references to use latest DLLs.
  * Small code clean-up.

### 1.0.0

  * Initial release.
