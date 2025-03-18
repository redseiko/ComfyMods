## Changelog

### 1.18.1

  * Hot-fix to only allow ColorPicker panel to be created in-game (and not on start-screen) to prevent NPE.

### 1.18.0

  * Fixed for the `v0.220.3` patch.
  * Experimental WIP ColorPicker panel added, can be toggled on/off with command `/toggle-color-picker`.

### 1.17.0

  * Updated for the `v0.218.21` patch.
  * Coloring pieces now uses vanilla `MaterialMan` game code logic instead of its own.
  * Known issue: wards/guard_stone will be completed colored instead of partially (to be fixed in a later update).

### 1.16.0

  * Updated for the `v0.218.19` patch.
  * Bumped up `<LangVersion>` to C# 12.
  * Code clean-up and refactoring.
  * Added new logic to prevent coloring chests/ships/wagons if they are currently in-use.
  * Simplified the change/remove-color prompt to not include `copyPieceColorShortcut` text.
  * Added a new `--position=<x,y,z>` arg to the `change-color` and `clear-color` commands.

### 1.15.1

  * Fixed a bug with `ClearPieceColorShortcut` not working.

### 1.15.0

  * Updated for the `v0.217.38` patch.
  * Moved changelog into `CHANGELOG.md`.
  * Bumped up `<LangVersion>` to C# 10.
  * Minor performance improvement by switching to singleton `PieceColorRenderer`.

### 1.14.0

  * Updated for `v0.217.31` patch.
  * Major code clean-up and restructuring.
  * Converted all commands to ComfyCommand format.
  * Added new commands `clear-color` and `change-color`.
  * Color palettes now wrap at 8 colors per row.

### 1.13.0

  * Fixed for the `v0.217.24` patch.

### 1.12.0

  * Fixed for `v0.217.14` patch.

### 1.11.1

  * Work-around for bug introduced in `v0.216.9` where Color.black (Vector3.zero) was stripped out during world load.

### 1.11.0

  * Updated for `v0.216.8` PTB patch.
  * Modified `Player.TakeInput()` transpiler to happen after `Player.UpdateHover()` and no longer block other inputs.
  * Modified all single-Piece color actions to no longer be coroutines.
  * Added work-around for new ZDO behaviour that does not support removal of existing ZDO key-value pairs.

### 1.10.0

  * Updated for the `v0.214.2` patch.

### 1.9.2

  * Reverted the 'Copied piece color' message back to an upper-left MessageHud notification.

### 1.9.1

  * Fixed a bug with the CopyPieceColorAction not working correctly and converted it to a regular method.
  * Organized some of the config files and other small clean-up.
  * Modified the `Player.TakeInput()` patch to also check that taking input is allowed.

### 1.9.0

  * Overhauled config options to use `ExtendedColorConfigEntry` for any color options.
  * Add ColorPalette feature for ExtendedColorConfigEntry.
  * Added `PieceLastColoredByHost` ZDO string field filled by player's `NetworkuserId` when modifying piece color.

### 1.8.0

  * Reduced overall memory and cpu usage!
  * Refactored entire colorization mechanism to use a new `PieceColor` component and `PieceColorUpdater` loop.
  * Removed prefab Material caching (which created instances) and instead make use of MaterialPropertyBlocks.
  * Added configuration options to override the Piece stability highlight gradient colors.
  * Cleaned-up this README and added more instructions.

### 1.7.1

  * Fixed a bug with the `Player.TakeInput()` transpiler code blocking other inputs.

### 1.7.0

  * Standardized `PluginConfig` to match more recent mods.
  * Moved Harmony patching code into new `PlayerPatch`, `TerminalPatch` and `HudPatch` classes.
  * Terminal commands now check for `IsModEnabled` for initial add and in the commands themselves.
  * Added `manifest.json` and changed `icon.png`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.6.0

  * Fixed crashes related to the VPO-compatibiity introduced in v1.4.0.
    * Reverted to original-caching behaviour that uses `WearNTear` instance itself as the key tied to Awake/Destroy.
    * Moved the SaveMaterialColor/ClearMaterialColor logic to WearNTearData.
    * Added a cache for Vector3ToColor() method calls.
    * Added a cache variable for Utils.ColorToVector3() method calls.

### 1.5.2

  * Changed how hotkeys are detected from Player.TakeInput() prefix to better Player.Update() transpiler.
    * This eliminates the double hot-key firing when in debugfly mode.
  * Moved more config-related logic into PluginConfig class.
  * Moved ZDO extensions to a new ZdoExtensions class.
  * Added two new Terminal.ConsoleCommands: /clearcolor and /changecolor

  - Fixed a missing check for isModEnabled and showChangeRemoveColorPrompt flags in Hud.UpdateCrosshair() postfix.
  - Fixed a missing yield return null condition in ChangeColorsInRadiusCoroutine().

### 1.4.0

  * Use `WearNTear.m_nview.m_zdo.m_uid` as the cache key for compatibility with ValheimPerformanceOptimizations.
  * Also call `ClearWearNTearColors()` in `WearNTear.Awake()` and `WearNTear.OnDestroy()` to assist with the above.

### 1.3.0

  * Fixed `PieceEmissionColorFactor` not being copied during copy color action.
  * Renamed `LastColoredBy` to `PieceLastColoredBy` to be more consistent with other colorful mods.
  * Added an option to change the font-size for the text prompt on hover.

### 1.2.1

  * Recompiled against H&H patch.

### 1.2.0

  * Fixed for Hearth & Home update.
  * Added new action to copy the (existing) color of the hovered piece.
    * Defaults to `LeftCtrl + R`.
  * All keyboard shortcuts for actions (including set color and clear color) are now configurable.
  * Increased maximum emission factor from `0.6` to `0.8` to allow for brighter colors.
  * Added a new `LastColoredBy` long ZDO property set to the PlayerId on set or clear.

## 1.1.0

  * Fixed a memory leak causing the game to crash/freeze during a player profile save.
    * This is because we used ConditionalWeakTable to cache piece materials but the Unity docs state that
      UnityEngine.Object does not support WeakReferences.
    * Changed to a Dictionary instead and patch `WearNTear.OnDestroy()` to remove the reference.
  * Added configuration setting to hide the 'change color' and 'remove color' prompt over a ward.

## 1.0.0

  * Initial release.