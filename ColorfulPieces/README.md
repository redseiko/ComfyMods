# ColorfulPieces

*Color any building piece made with the hammer using RGB/HTML colors!*

## Features

  * You can color any building piece that can be built using the Hammer using RGB or HTML color codes.
  * Coloring is very simple at the moment and will color all materials/textures on the object (to be refined later).
  * Those without the mod installed will still see the default vanilla materials/textures. 

## ColorPicker

  * A new interface for selecting a color!

    ![ColorPicker & ColorPalette](https://i.imgur.com/ZSDonUOl.png)

  - Toggle the ColorPicker by:
    - Using the command `/toggle-color-picker`
    - Using a key-bind by setting the `toggleColorPickerShortcut` config-option.
    - Press `Escape` when the ColorPicker is open to close it.

  * Using the ColorPalette:
    * Add the **current color** to the palette using the "Add" button.
    * Remove the **last palette color** using the "Remove" (trashcan) button.
    * __Click__ on a palette color to select that color and **__close the ColorPicker panel__**.
    * __Hold__ `LeftShift` and click on a palette color to select that color.
    * __Hold__ `LeftControl` and click on a palette color **__to delete that specific color__** from the palette.

## Instructions

### Setting target color

  * In-game, press `F1` to bring up the `ConfigurationManager` and navigate to the ColorfulPieces section.
  * Change the target color using the RGB sliders or using an HTML color code.
  * Change the target emission color factor using the slider (this affects how bright the target color will be).

### Changing piece colors

  * Hover over any building piece ***that you are the owner of*** and a prompt will appear.
  * Hit `LeftShift + R` to change the building piece color to the target color and emission factor.
  * Hit `LeftAlt + R` to clear any existing colors from the building piece.
  * Hit `LeftCtrl + R` to copy the existing color from a piece.

  - This prompt can be hidden by disabling the `showChangeRemoveColorPrompt` setting.
  - Prompt font-size can be configured with the `colorPromptFontSize` setting.

## Configuration

  * Configuration options can be accessed in-game using
    [ConfigurationManager](https://thunderstore.io/c/valheim/p/Azumatt/Official_BepInEx_ConfigurationManager/).

    ![Configuration Options](https://i.imgur.com/f9PLswAl.png)

## Commands

### Changing/Clearing pieces in a radius (legacy)

These two commands still call the same action as the hotkey and so will obey all ward permissions.

  * `/clearcolor <radius>` (in chatbox)
  * `clearcolor <radius>` (in console)
  * Clears any colors from all pieces in the specified radius from the player.

  - `/changecolor <radius>` (in chatbox)
  - `changecolor <radius>` (in console)
  - Changes the color of all pieces in the radius from the player to the currently set target color.

### Clearing piece colors in a radius

    clear-color --radius=<radius> [--prefab=<name1>] [--position=<x,y,z>]
    clear-color --r=<radius> [--p=<name1>] [--pos=<x,y,z>]

  * Clears any colors from pieces within `<radius>` meters from the playerr.
  * If `--prefab` is specified, only pieces with prefab names matching `<name1>` will be affected.
  * Specify multiple prefab names with commas: `--prefab=<name1>,<name2>`
  * If `--position` is specified, target position will be at `x,y,z` instead of the current player position.

### Changing piece colors in a radius

    change-color --radius=<radius> [--prefab=<name1>] [--position=<x,y,z>]
    change-color --r=<radius> [--p=<name1>] [--pos=<x,y,z>]

  * Changes the color of pieces within `<radius>` meters from the player to the currently set target color.
  * If `--prefab` is specified, only pieces with prefab names matching `<name1>` will be affected.
  * Specify multiple prefab names with commas: `--prefab=<name1>,<name2>`
  * If `--position` is specified, target position will be at `x,y,z` instead of the current player position.

## Notes

  * See source at: [GitHub/ComfyMods](https://github.com/redseiko/ComfyMods/tree/main/ColorfulPieces).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
