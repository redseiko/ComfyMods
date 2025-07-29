## Changelog

### 1.8.0

  * Added new config-section `SectorInfoPanel.CopyPosition` with config-options:
    * `copyPositionValuePrefix`
    * `copyPositionValueSeparator`
    * `copyPositionValueOrder`
  * Modified the behaviour of the "Copy" button to use the new config-options above.
  * Updated icon.

### 1.7.0

  * Updated for the `v0.220.5` patch.
  * Removed obsolete patch-compatibility code.
  * Now using `AssetID` to fetch the `Custom/Distortion` shader used for sector-boundaries.
  * Sector-boundaries cube now adjusts for player's `position.y` value.
  * Code clean-up and refactoring.

### 1.6.0

  * Fixed for the `v0.219.10` PTB patch.
  * Bumped up `<LangVersion>` to C# 12.
  * Code clean-up and refactoring.

### 1.5.0

  * Added new config option `toggleSectorBoundariesShortcut` to use to toggle sector boundaries (default: un-set).

### 1.4.0

  * Updated for `v0.217.38` patch.
  * Moved changelog into `CHANGELOG.md`.
  * Added new `Copy` button to copy current position to clipboard.

### 1.3.0

  * Updated all `UI.Text` references to use `TextMeshPro`.
  * Added new config option `showZDOManagerContent` to toggle `NextId` row in the `SectorInfoPanel`, default to off.

### 1.2.0

  * Fixed for `v0.216.9` patch.

### 1.1.0

  * Added a new row `ZdoManager.NextId` to the `SectorInfoPanel`.
  * Minor code clean-up.

### 1.0.1

  * Fixed grid display of sectors corresponding to NESW movement. Blah math.

### 1.0.0

  * Initial release.
