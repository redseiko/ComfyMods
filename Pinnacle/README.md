# Pinnacle

*Pinnacle perpetually provides premium pin performance.*

![Pinnacle - At a Glance](https://imgur.com/Wabfnru.png)

## New Feature

  * You can now use a keyboard shortcut for adding a Pin!
  * Set the shortcut in ConfigurationManager under `Minimap.Actions` > `addPinAtMouseShortcut`.

## Features

### PinEditPanel

  * Edit existing pins and add new pins with more icon types!

    ![Pinnacle - Edit an Existing Pin](https://imgur.com/ODB2jVz.png)

  - Edit an existing pin
    - Left-click on an existing pin on the map.
    - The *PinEditPanel* will toggle on with the default pin data.
    - You can modify the pin name, icon and position.
    - You can toggle the pin checked state and shared state.

  * Add a new pin
    * Left-double-click on the target point on the map to add a new pin.
    * The *PinEditPanel* will toggle on with default pin data.

  - Click anywhere on the map without a pin to toggle off the *PinEditPanel*.

### PinListPanel

  * Lists all your pins or filter them by pin name!

    ![Pinnacle - Show the PinListPanel](https://imgur.com/IrE36jV.png)

  - Show/hide the *PinListPanel*
    - Press `Tab` (configurable) to toggle the *PinlistPanel* on and off.
    - All pins will initially be listed and pin count shown on the bottom.

  * Filter pins by name
    * Enter text in the input field at the top of the panel.
    * Pins will by filtered by matching text in their name.

  - Center map on pin
    - Left-click on the target pin row and the map will center onto that pin
    - Scrolling animation can be disabled by setting `CenterMap.lerpDuration` config to 0.

  * Reposition the *PinListPanel*
    * Left-click on an open space on the panel and drag to reposition.

  - Resize the *PinListPanel*
    - Hover near the **bottom-right corner** to show the resize button.
    - Left-click and drag on this button to resize the panel.

### PinFilterPanel

  * Filter pins on the map by **any icon type** (replaces vanilla panel).

    ![Pinnacle - PinFilterPanel](https://imgur.com/fPs7fDd.png)

### Map Teleporting

  * **Requirements!**
    * `devcommands` must be enabled via console.
    * You must be in single-player mode or the local server-host.

  - Teleport to map point
    - Hold `LeftShift` and click on the target point on the map.

  * Teleport to pin position
    * Hold `LeftShift` and click on the target row in the *PinListPanel*.

### Configuration

  * Important/critical configuration options are available (more to come later).

    ![Pinnacle - Configuration](https://imgur.com/DBUH4Jq.png)

  - Changing the Minimap.Pin font/font-size
    - These two options are available once you are logged into any world.

## Notes

  * See source at: [GitHub](https://github.com/redseiko/ComfyMods/tree/main/Pinnacle).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
  * Pinnacle icon created by [@jenniely](https://twitter.com/jenniely) (jenniely.com)

## Changelog

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