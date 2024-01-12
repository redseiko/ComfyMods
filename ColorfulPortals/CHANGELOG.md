## Changelog

### 1.8.0

  * Updated for `v0.217.38` patch.
  * Moved changelog into `CHANGELOG.md`.

### 1.7.0

  * Fixed for `v0.217.14` patch.
  * Removed patch logic for wiring stone portals as they now work correctly in vanilla.
  * Reworkd portal coloring logic for new `ParticleSystem.CustomData` module.
  * `TargetPortalColor` now uses new `ExtendedColorConfigEntry` used in other colorful mods.
  * Modified the keyboard shortcut logic to prevent further keypress if a the action was performed.

### 1.6.2

  * Actually create `TeleportWorldColorUpdater` to update portal colors on a loop.
  * Added `updateColorsFrameLimit` and `updateColorsWaitInterval` to control this behaviour.
  * Updated mod icon to a new one created by [@jenniely](https://twitter.com/jenniely).

### 1.6.1

  * Removed color support for stone portals for now to simplify mod logic and improve performance.
  * Added work-around for Color.black/Vector3.zero value being stripped out of portal colors on world load.
  * Minor code-refactoring.

### 1.6.0

  * Fixed for `v0.216.9` patch.
  * Modified `Player.TakeInput()` transpiler patch to happen after `Player.UpdateHover()`.
  * Modified `ChangePortalColor` to no longer be a coroutine.
  * Created WIP `TeleportWorldColor` component to use for regular portals, stone portal to be updated later.

### 1.5.0

  * Moved change color code from `TeleportWorld.Interact()` prefix to `Player.TakeInput()` transpiler with coroutine.
    * Can now configure the hot-key to change portal color.
  * Changed some of the logic in `RemovedDestroyedTeleportWorldsCoroutine()`.
  * Removed configuration option for `colorPromptFontSize` (UI overhaul coming later).
  * Extracted configuration options into new `PluginConfig` class.
  * Extracted `TeleportWorldData` into its own class.
  * Added `manifest.json`, `icon.png` and updated this `README.md`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.4.0

  * Added an option to change the font-size for the text prompt on hover.

### 1.3.0

  * Updated for Hearth & Home.
  * Added `PortalLastColoredBy` ZDO property that is set whenever a player changes the portals color.

### 1.2.0

  * Fixed a memory-leak when caching TeleportWorld/Portals.

### 1.1.0

  * Adding configuration setting to hide the 'change color' prompt over a ward.
  * Now saves the target color's **alpha** value to the ZDO and reads/uses this alpha value if present in the ZDO.

### 1.0.0

  * Initial release.
