# EulersRuler

  * Shows targeted piece name, health, stability and rotatios while hammer is active
  * Shows name and rotation of build piece currently being placed.
  * Show (or hide) the vanilla piece health bar (colorized to the current piece health).

## User Interface

  * This fuctionality is available while the build hammer is active.
  * Display a panel with the piece name, health, stability and rotation (Euler & Quaternion) of the targeted piece.
    * Panel position can be configured.
    * Every row can be toggled on/off in the configuration.
  * Display a panel with the prefab name and rotation (Euler & Quaternion) of the placement ghost piece.
    * Panel position can be configured.
    * Every row can be toggled on/off in configuration.

## Instructions

  1. Extract `EulersRuler.dll` to your `/Valheim/BepInEx/plugins/` folder.
  2. Activate building mode by making your hammer your active weapon.

## Known Conflicts

  * BuildingHealthDisplay
    * Mod functionality is duplicated, disable one or the other.

## Changelog

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