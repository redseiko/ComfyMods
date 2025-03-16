## Changelog

### 1.5.0

  * Fixed for the `v0.220.3` patch.
  * Moved changelog into `CHANGELOG.md`.
  * Bumped up `<LangVersion>` to C# 12.
  * Code clean-up and refactoring.

### 1.4.0

  * Fixed for the `v0.217.24` patch.

### 1.3.0

  * Extracted patch and config code into separate classes.
  * Removed async-saving of player profile to play nice with Unity threading.
  * Modified mod save logic to **only save player profile** (and not the world file) during a mod save.
  * Moved the `compress map data` feature to be behind a toggle in config.

### 1.2.0

  * Added `manifest.json`, `icon.png` and `README.md`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.1.1

  * Minor fixes to the UI.

### 1.1.0

  * Moved to compression-only of map data to utilized newly added compression in H&H update.

### 1.0.0

  * Added compression/decompression of map data for player profiles.
