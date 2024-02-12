# Intermission

*Show custom images and tips on the loading screen.*

## Instructions

### Adding custom loading images and loading tips

#### Thunderstore

  * Go to Settings > Browse profile folder.
  * Navigate to the `BepInEx > config > Intermission` folder.
  * Place `.png` or `.jpg` image files in this folder.
  * Create (or modify) `tips.txt` textfile in this folder, one loading tip per line.

  ![Thunderstore Example](https://i.imgur.com/P1MY0X8.png)

#### Manual

  * Navigate to your `Valheim > BepInEx > config > Intermission` folder.
  * Place `.png` image files in here.
  * Create (or modify) `tips.txt` textfile in this folder, one loading tip per line.

  ![Manual Example](https://i.imgur.com/mWaJrIi.png)

## Create a content mod for Thunderstore

  * In your `manifest.json` add a dependency to `ComfyMods-Intermission-1.4.0`.
  * Structure your content mod zip file to resemble the following example:

  ```
  # AuthorName-ContentModName.zip
    - [config]
      - [Intermission]
        - image1.png
        - image2.png
        - image3.jpg
        - tips.txt
    - icon.png
    - manifest.json
    - README.md
  ```

## Configuration

  * Changes to all settings will take effect immediately (except for `_isModEnabled` which requires a game restart).
  * Use [ConfigurationManager](https://valheim.thunderstore.io/package/Azumatt/Official_BepInEx_ConfigurationManager/)
    to modify settings in-game.

  ![Configuration](https://i.imgur.com/ifxhwFJ.png)

## Installation

### Manual

  * Un-zip `/config/` and `/plugins/` folders into your `/Valheim/BepInEx/` folder.

### Thunderstore (manual)

  * Go to Settings > Import local mod > Select `Intermission_v1.4.0.zip`.
  * Click "OK/Import local mod" on the pop-up for information.

## Notes

  * See source at: [GitHub/ComfyMods](https://github.com/redseiko/ComfyMods/tree/main/Intermission).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
