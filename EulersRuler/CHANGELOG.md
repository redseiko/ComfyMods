## Changelog

### 1.8.0

  * Updated for the `v0.218.19` patch.
  * Code clean-up and refactoring.

### 1.7.0

  * Added a new enum `PieceName` to the `hoverPiecePanelEnabledRows` and `placementGhostPanelEnabledRows`.
  * When enabled, property panels will show the target Piece's `GameObject.name` next to the localized `Piece.m_name`.

### 1.6.0

  * Updated for the `v0.217.38` patch
  * Moved changelog into `CHANGELOG.md`.
  * Bumped up `<LangVersion>` to C# 10.
  * Code clean-up and refactoring.

### 1.5.0

  * Migrated to `TextMeshPro`.
  * Some code refactoring and clean-up (more to be done later).
  * Simplified `HoverPiece.HealthBar` patch logic.
  * Added a splash image to this `README.md`.

### 1.4.0

  * Updated for the `v0.214.2` patch.

### 1.3.0

  * Added `manifest.json`, changed the `icon.png` and updated this `README.md`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.2.0

  * Updated for Hearth & Home.
  * Increased decimal points for Quaternion values to 2.

### 1.1.1

  * Fixed a `NullReferenceException` when the player deconstructs the current hovered piece.

### 1.1.0

  * Added configuration for properties text font size.
  * Added configuration to hide or show the vanilla piece health bar.
  * Health bar colorized to match piece health gradient.
  * Health and stability current values colorized to match their percent gradient color.
  * Update HoverPiece and PlacementGhost code moved to coroutine to only update 4x/second instead of every frame.

### 1.0.0

  * Initial release.
