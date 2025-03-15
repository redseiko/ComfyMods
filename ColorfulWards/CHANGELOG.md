## Changelog

### 1.7.0

  * Fixed for the `v0.220.3` patch.
  * Moved changelog into `CHANGELOG.md`.
  * Bumped up `<LangVersion>` to C# 12.
  * Code clean-up and refactoring.

### 1.6.0

  * Fixed for the `v0.217.14` patch.
  * Extracted all patch logic into separate classes.
  * Created new `PrivateAreaColor` component to encapsulate and simplify ward coloring logic.
  * Rewrote the `PrivateArea.IsInside()` from a prefix-patch to transpiler.
  * `TargetWardColor` now uses new `ExtendedColorConfigEntry` used in other colorful mods.
  * Modified the keyboard shortcut logic to prevent further keypress if a the action was performed.

### 1.5.0

  * Updated for the `v0.216.9` patch.
  * Modified `Player.TakeInput()` transpiler to happen after `Player.UpdateHover()` and no longer block other inputs.
  * Modified `ChangeWardColor` to no longer be a coroutine.

### 1.4.1

  * Fixed a bug with the `Player.TakeInput()` transpiler blocking other inputs with the same keybind.

### 1.4.0

  * Moved change color code from `PrivateArea.Interact()` prefix to `Player.TakeInput()` transpiler with coroutine.
    * Can now configure the hot-key to change ward color.
  * Removed configuration option for `colorPromptFontSize` (UI overhaul coming later).
  * Extracted configuration options into new `PluginConfig` class.
  * Added `manifest.json`, `icon.png` and updated this `README.md`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.3.0

  * Updated for Hearth & Home.
  * Added `WardLastColoredBy` ZDO tag that is set to the last Player that modifies the Ward color.
  * Added an option to change the font-size for the text prompt on hover.

### 1.2.0

  * Fixed a memory-leak when caching PrivateArea/Wards.

### 1.1.0

  * Adding configuration setting to hide the 'change color' prompt over a ward.
  * Now saves the target color's **alpha** value to the ZDO and reads/uses this alpha value if present in the ZDO.
    * However, there isn't any apparent effect/use for color alpha for ward lights and particle systems.'

### 1.0.0

  * Initial release.