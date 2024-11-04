## Changelog

### 1.7.0

  * Fixed for the `v0.219.14` patch.
  * Moved changelog into `CHANGELOG.md`.
  * Bumped up `<LangVersion>` to C# 12.
  * Code clean-up and refactoring.

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