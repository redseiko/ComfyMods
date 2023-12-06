# SearsCatalog

*Resize and reposition the build panel in-game.*

![Splash](https://i.imgur.com/jwYCsdf.png)

## Features

### Panel resize (mouse)

  * Hover over the lower-right corner of the build panel to display the resize icon.
  * Click and drag the resize icon to the desired size.
  * Build panel will resize to the closest `Rows x Columns` size.

### Panel resize (config)

  * Using ConfigurationManager, change `buildHudPanelRows` or `buildHudPanelColumns` to desired values.
  * Build panel will resize to the new `Rows x Columns` size immediately.

### Panel reposition

  * Click on any empty space on the build panel and drag it to the desired position.
  * Position is saved/loaded from config and can be reset in ConfigurationManager.

## Compatability

  * SearsCatalog is a replacement for ComfyBuildExpansion and will try to unpatch CBE.
  * To avoid any issues, please uninstall or disable ComfyBuildExpansion.

## Configuration

All configuration options (except `isModEnabled`) can be modified in-game and will take effect immediately.

![Configuration](https://i.imgur.com/Faoihs5.png)

## Installation

### Manual

  * Un-zip `SearsCatalog.dll` to your `/Valheim/BepInEx/plugins/` folder.

### Thunderstore (manual)

  * Go to Settings > Import local mod > Select `SearsCatalog_v1.4.0.zip`.
  * Click "OK/Import local mod" on the pop-up for information.

## Notes

  * See source at: [GitHub](https://github.com/redseiko/ComfyMods/tree/main/SearsCatalog).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
  * SearsCatalog icon created by [@jenniely](https://twitter.com/jenniely) (jenniely.com)

## Changelog

### 1.4.0

  * Updated for `v0.217.31` patch.
  * Fixed a bug with mouse scroll-wheel scrolling through category tags instead of the build panel pieces.
  * Fixed a bug with selecting the correct build piece when using the vanilla 'copy piece' function.
  * Removed logic related to un-patching deprecated BuildExpansion if detected.
  * Minor code clean-up.

### 1.3.0

  * Updated for `v0.217.14` patch.

### 1.2.0

  * Updated for `v0.214.2` PTB.
  * Updated mod icon to a new one created by [@jenniely](https://twitter.com/jenniely).

### 1.1.0

  * Added `PieceTable.Left/Right/Up/DownPiece()* patches for controller support.
  * Added `PanelResizer` and a resizer icon to the build panel.

### 1.0.0

  * Initial release.