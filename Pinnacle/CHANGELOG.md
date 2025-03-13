## Changelog

### 1.12.0

  * Fixed for the `v0.220.3` patch.

### 1.11.0

  * Fixed for the `v0.219.14` patch.
  * Bumped up `<LangVersion>` to C# 12.
  * Added a "Refresh" button to the `PinListPanel`.
  * Modified `PinListPanel` scrolling behaviour from elastic to clamped.
  * Added config-option `PinListPanel.ScrollRect > movementType` to allow switching between scrolling behaviour.
  * Adding new pins will now try to calculate the position `Y` value from the Heightmap.
  * Code clean-up and refactoring.

### 1.10.0

  * Removed custom Player pin creation added in `v1.2.1` as vanilla game no longer orphans Player pins.

### 1.9.0

  * Updated for the `v0.217.46` patch.
  * Bumped up `<LangVersion>` to C# 10.
  * Code clean-up and refactoring.

### 1.8.0

  * Extracted changelog into `CHANGELOG.md`.
  * Added config option `PinListPanelShowPinPosition` to toggle the `Pin.Position` columnns in the `PinListPanel`.
  * Added config option `PinListPanelEditPinOnRowClick` to show the `PinEditPanel` when clicking on a row in the
    `PinlistPanel`.
  * Adjusted `PinListPanel` to be a bit more compact.

### 1.7.0

  * Added a new feature to add a Minimap.Pin using keyboard shortcut.

### 1.6.0

  * Updated for `v0.217.31` patch.
  * Fixed a bug where `Pin.Font` and `Pin.FontSize` was not correctly applied to the `PinMarker.PinName` prefab.
  * Converted all Terminal commands to `ComfyCommand` format.
  * Removed temporary debugging `pinnacle-namepindata-clearall` Terminal command.
  * Pin Import/Export commands now read and write the new `PinData.m_author` field.

### 1.5.3

  * Fixed a bug in `PinListPanel.SetTargetPins()` where it was not fetching Heightmap info for pins with Y values == 0.
  * Modified `Pinnacle.TeleportTo` to fetch Heightmap info when targetPosition.y == 0.

### 1.5.2

  * Removed the `targetPosition.y` override in the `TeleportTo()` helper method as this is now handled by vanilla code.

### 1.5.1

  * Fixed a bug where you could not add a new pin if the 'Death' pin icon was selected.
  * Changed permission level for `resetmap` command to be useable without cheats.
  * Minor code refactoring.

### 1.5.0

  * Fixed for the `v0.217.22` patch.

### 1.4.1

  * Added `Game.UpdateNoMap()` transpiler patch to fix for a vanilla bug where the minimap will close whenever global
    keys are received.
  * Refactored logging methods to also log to chat.

### 1.4.0

  * Fixed for `v0.217.14` patch.
  * Migrated all `UI.Text` usage to new `TMP_Text`.
  * PinListPanel: pin names for pins with no name will be italicized.
  * PinListPanel: pin names starting with '$' will be localized for display.
  * Fixed Pin name font and font-size wiring for TMP_Text, limited to built-in TMP_FontAssets only (for now).

### 1.3.0

  * Fixed build errors for `v0.216.9` PTB update.
  * Added new project reference to `ui_lib_publicized.dll`.
  * Converted `Minimap.ShowPinNameInput()` from postfix-patch to prefix-patch due to signature/behaviour change.

### 1.2.4

  * Re-wrote the entire `Minimap.UpdatePlayerPins()` prefix patch again to better update player pins.

### 1.2.3

  * Hot-fix for the hot-fix because I forgot to specify gameObject for destroy.

### 1.2.2

  * Hot-fix for orphaned player pin names: rewrote entire `Minimap.UpdatePlayerPins()` with prefix patch.

### 1.2.1

  * Fixed a bug where pin name text was not removed when pins were removed.
  * Temporary hot-fix for player pin names not updating/moving until you zoomed all the way out.
  * Updated BepInEx dependency to `denikson-BepInExPack_Valheim-5.4.2100`.

### 1.2.0

  * Updated for the `v0.214.2` patch.
  * Added new prototype `UIBuilder.CreateSuperellipse()` for more rounded panels.
  * Updated mod icon to a new one created by [@jenniely](https://twitter.com/jenniely).

### 1.1.0

  * Added new commands for exporting pins to a file in binary or text format.
  * Added new command for import pins from a binary file.
  * Refactored plugin configuration code.

### 1.0.3

  * Fixed a bug where the PinEditPanel would not default the Pin.Icon to the last selected one.

### 1.0.2

  * Fixed label for the Z-value in VectorCell incorrectly showing 'X'.
  * Removed code in `UIBuilder.CreateRoundedCornerSprite()` that saved the sprite to disk, was used for debugging.

### 1.0.1

  * Fixed a bug where the *PinEditPanel* was blocking map-movement when toggled off.

### 1.0.0

  * Initial release.
