# Silence

  * Creator tool to completely hide the chat windows and in-game say/shout with a shortcut.

## User Interface

  * Hide the chat window and in-world texts with: `RightCtrl + S` (configurable)
  * Hiding only the chat window or only the in-world texts can be configured in ConfigurationManager.

## Instructions

  1. Extract `Silence.dll` to your `/Valheim/BepInEx/plugins/` folder.
  2. Chat and shouts are shown by default, use `RightCtrl + S` to toggle silence.

## Notes

  * You can also download the mod at:
    * Nexus Mods: n/a
    * Thunderstore: n/a

## Changelog

### 1.1.0

  * Updated for Hearth & Home.
  * Fixed a possible issue with the toggle shortcut check in `Player.TakeInput()`.
  * Fixed the ChatWindow popping up with a shout when Silence is turned on, because Chat inherits from Terminal now.

### 1.0.0

  * Initial release.