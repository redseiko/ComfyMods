## Changelog

### 1.11.0

  * Fixed for the `v0.217.38` patch.
  * Moved changelog into `CHANGELOG.md`.

### 1.10.0

  * Fixed for `v0.217.14` patch.
  * Removed colored fireworks feature as it broke with new vanilla fireworks items.
  * Modified the keyboard shortcut logic to prevent further keypress if a the action was performed.

### 1.9.0

  * Fixed for `v0.216.8` PTB patch.
  * Modified `Player.TakeInput()` transpiler patch to happen after `Player.UpdateHover()`.
  * Minor code clean-up.

### 1.8.0

  * Converted color config option to use `ExtendedColorConfigEntry` and `ColorPalette`.
  * Added `PieceLastColoredByHost` ZDO string field filled by player's `NetworkuserId` when modifying light color.
  * Modified the `Player.TakeInput()` patch to also check that taking input is allowed.

### 1.7.1

  * Fixed yet another small bug with the `Player.TakeInput()` transpiler not matching the same pattern as other mods.

### 1.7.0

  * Fixed a bug with the `Player.TakeInput()` transpiler blocking other inputs with the same keybind.
  * Rewrote the entire fireplace-coloring system to use a new MonoBehaviour `FireplaceColor`.
  * Fixed colorizing fireworks... it never worked properly before and now it does.
  * Moved patch-related code into their own classes.

### 1.6.0

  * Move action check from `Fireplace.Interact()` prefix to Player.TakeInput() transpiler delegate.
    * Can now configure the hot-key to change the color.
  * Convert spawn colored fireworks code from `Fireplace.UseItem()` prefix to a transpiler.

### 1.5.0

  * Added an option to change the font-size for the text prompt on hover.

### 1.4.0

  * Updated for Hearth & Home.
  * Renamed `LastColoredByPlayerId` ZDO key to `LightLastColoredBy`.

### 1.3.0

  * Fixed colors not applying to the Light component in the `Point light` GameObject.
  * Now writes the PlayerId to a new ZDO field: `LastColoredByPlayerId`.

### 1.2.0

  * Fixed a memory leak when caching lights/fires. Now starts a coroutine to clean-up the cache.

### 1.1.0

  * Adding configuration setting to hide the 'change color' prompt over a torch or fireplace.
  * Now saves the target color's **alpha** value to the ZDO and reads/uses this alpha value if present in the ZDO.

### 1.0.0

  * Initial release.
