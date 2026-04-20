## Changelog

### 1.5.0

  * Updated for the `v0.221.12` patch.
  * Migrated to SDK-style project.
  * Code refactor and clean-up.
  * Updated mod icon.

### 1.4.0

  * Fixed for `v0.216.9` patch.
  * Modified `ReadHiddenText()` to no longer be a coroutine.

### 1.3.0

  * Fixed for the `v0.214.2` patch.
  * Changed the `Player.TakeInput()` delegate to a `Player.UpdateHover()` delegate with better key-down handling.

### 1.2.0

  * Fixed the `Player.Update()` transpiler TakeInput delegate to properly work with other mods that also patch it.
  * Extracted configuration-related code into a new `PluginConfig` class.
  * Extracted extension methods into a new `PluginExtensions` class.
  * Extracted patch-related code into new `PlayerPatch` and `HudPatch` classes.
  * Added `manifest.json`, `icon.png` and this `README.md`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.1.0

  * New support for Inscriptions `v1.1.0` in enabling inscriptions for **anything**.
  * Moved hover text modification to `Hud.UpdateCrosshair()` to support the new feature.

### 1.0.0

  * Initial release.
