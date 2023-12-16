# Silence

*Completely hide the chat windows and in-game say/shout with a shortcut.*

## Instructions

### User Interface

  * Hide the vanilla chat window and in-world texts with: `RightCtrl + S` (configurable).
  * Hiding only the chat window or only the in-world texts can be configured in ConfigurationManager.

  - Enabling or disabling the mod with `isModEnabled` requires a **restart** of the game to take effect.

## Compatability

### Chatter

  * Conflicts with `Chatter` mod as `Chatter` disables the vanilla chat window.
  * If using `Chatter` ensure that the `HideChatWindow` configuration option is **disabled**.

### Notes

  * See source at: [GitHub](https://github.com/redseiko/ComfyMods/tree/main/Silence).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)

## Changelog

### 1.6.0

  * Fixed for the `v0.217.38` patch.

### 1.5.0

  * Reworked `ToggleSilenceCoroutine()` to use a single `IsSilenced` toggle and to use the `HideChatWindow` and
    `HideInWorldTexts` config values to toggle the chat-window / in-world texts.
  * Added compatability section for `Chatter`.

### 1.4.0

  * Fixed for the `v0.217.14` patch.

### 1.3.0

  * Moved all configuration code into new `PluginConfig` class.
  * Moved all Harmony-patching code into their own patch classes.
  * Modified the `Chat.Update()` transpiler to insert a new instruction instead of overwriting the existing one.
  * Fixed the `Player.Update()` transpiler TakeInput delegate to properly work with other mods that also patch it.
  * Added `manifest.json`, changed the `icon.png` and updated this `README.md`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.2.0

  * Modified the check for the keyboard shortcut to use a transpiler on `Player.Update()` instead of `TakeInput()`.

### 1.1.0

  * Updated for Hearth & Home.
  * Fixed a possible issue with the toggle shortcut check in `Player.TakeInput()`.
  * Fixed the ChatWindow popping up with a shout when Silence is turned on, because Chat inherits from Terminal now.

### 1.0.0

  * Initial release.