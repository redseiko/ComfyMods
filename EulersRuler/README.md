# EulersRuler

*Show building piece health, stability and rotation on your HUD!*

![EulersRuler v1.5.0 - Splash](https://imgur.com/TOLdJ8n.png)

## Features

  * Shows targeted piece name, health, stability and rotations while hammer is active
  * Shows name and rotation of build piece currently being placed.
  * Show (or hide) the vanilla piece health bar (colorized to the current piece health).

## User Interface

This functionality is available while the build hammer is active.

  * Display a panel with the piece name, health, stability and rotation (Euler & Quaternion) of the targeted piece.
    * Panel position can be configured.
    * Every row can be toggled on/off in the configuration.

  - Display a panel with the prefab name and rotation (Euler & Quaternion) of the placement ghost piece.
    - Panel position can be configured.
    - Every row can be toggled on/off in configuration.

## Known Conflicts

  * BuildingHealthDisplay
    * Mod functionality is duplicated, disable one or the other.

### Notes

  * See source at: [GitHub](https://github.com/redseiko/ComfyMods/tree/main/EulersRuler).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
  * Check out our community driven listing site at: [valheimlist.org](https://valheimlist.org/)

## Changelog

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