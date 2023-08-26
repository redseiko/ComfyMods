# Shortcuts

*Re-assign more keyboard shortcuts that are hard-coded in vanilla.*

## Features

### Configurable Keys!

| Section      | Shortcut                   | Default         |
|--------------|----------------------------|-----------------|
| Console      | toggleConsoleShortcut      | `F5`            |
| Debugmode    | debugKillAllShortcut       | `None`          |
| Debugmode    | debugRemoveDropsShortcut   | `None`          |
| Debugmode    | toggleDebugFlyShortcut     | `Z`             |
| Debugmode    | toggleDebugNoCostShortcut  | `B`             |
| GameCamera   | takeScreenshotShortcut     | `F11`           |
| GameCamera   | toggleMouseCaptureShortcut | `LeftCtrl + F1` |
| Hud          | toggleHudShortcut          | `LeftCtrl + F3` |
| ConnectPanel | toggleConnectPanelShortcut | `F2`            |
| Hotbar       | hotbarItem1Shortcut        | `1`             |
| Hotbar       | hotbarItem2Shortcut        | `2`             |
| Hotbar       | hotbarItem3Shortcut        | `3`             |
| Hotbar       | hotbarItem4Shortcut        | `4`             |
| Hotbar       | hotbarItem5Shortcut        | `5`             |
| Hotbar       | hotbarItem6Shortcut        | `6`             |
| Hotbar       | hotbarItem7Shortcut        | `7`             |
| Hotbar       | hotbarItem8Shortcut        | `8`             |

### Changes from default

  * Un-binds the debugmode KillAll shortcut `K` by default because of many accidental pet deaths. :(
  * Un-binds the debugmode RemoveAll shortcut `L` by default.

## Installation

### Manual

  * Un-zip `Shortcuts.dll` to your `/Valheim/BepInEx/plugins/` folder.

### Thunderstore (manual)

  * Go to Settings > Import local mod > Select `Shortcuts_v1.4.0.zip`.
  * Click "OK/Import local mod" on the pop-up for information.

## Notes

  * See source at: [GitHub](https://github.com/redseiko/ComfyMods/tree/main/Shortcuts).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
  * Check out our community driven listing site at: [valheimlist.org](https://valheimlist.org/)

## Changelog

### 1.4.0

  * Updated code match references for Input class to ZInput class for compatibility with patch 0.217.14.

### 1.3.0

  * Updated for `v0.214.2` PTB.
  * Replaced the special `IsDown()` with a simpler method that uses `Input.GetKey()/GetKeyDown()`.

### 1.2.0

  * Prototype using a special version of `IsDown` modified from BepInEx's KeyboardShortcut code.
  * Clean-up some of the transpiler delegate code and the PluginConfig code.
  * Added `manifest.json`, `icon.png` and `README.md`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.1.0

  * Updated for Hearth and Home.
  * Added toggles for:
    * Debugmode - removedrops `L`
    * ConnectPanel - toggle `F2`

### 1.0.0

  * Initial release.