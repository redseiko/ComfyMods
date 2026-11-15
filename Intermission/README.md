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

### Loading Tips

  * You can use `\n` for newline as well as TextMeshPro rich text tags.
  * See https://docs.unity3d.com/Packages/com.unity.ugui@7.0/manual/TextMeshPro/RichText.html for documentation.

## Create a content mod for Thunderstore

  * In your `manifest.json` add a dependency to `ComfyMods-Intermission-1.10.0`.
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

## Installation

### Manual

  * Un-zip `/config/` and `/plugins/` folders into your `/Valheim/BepInEx/` folder.

### Thunderstore (manual)

  * Go to Settings > Import local mod > Select `Intermission_v1.10.0.zip`.
  * Click "OK/Import local mod" on the pop-up for information.

## Configuration

  * Changes to all settings will take effect immediately (except for `isModEnabled` which requires a game restart).
  * Use [BepInEx ConfigurationManager](https://thunderstore.io/c/valheim/p/ComfyDeps/BepInExConfigurationManager/) to
    modify settings in-game.

`[_Global]`

  * `isModEnabled`
    * Globally enable or disable this mod (restart required).
    * Default: `true`

`[Hud.LoadingScreen]`

  * `showImmediatelyOnDeath`
    * If set, will show the loading screen immediately on death.
    * Default: `true`
  * `showOnTeleport`
    * If set, will show loading screen instead of vanilla teleport animation.
    * Default: `true`

<details>
<summary>Other Configuration Options (<i>click to expand</i>)</summary>
<br />

`[SceneLoader]`

  * `useLoadingImages`
    * If set, will use custom loading images on the initial `SceneLoader` scene.
    * Default: `true`
  * `showProgressText`
    * If set, will show loading progress text on the initial `SceneLoader` scene.
    * Default: `true`
  * `centerProgressIndicator`
    * If set, will center the loading progress indicator (instead of being on the lower-right).
    * Default: `true`
  * `progressIndicatorOffset`
    * When `centerProgressIndicator` is `true`, this is used to offset from the bottom center.
    * Default: `(0, 200)`

`[LoadingImage.Image]`

  * `baseColor`
    * The base color to apply to the loading image.
    * Default: `#FFFFFFFF`

`[LoadingImage.ScaleLerp]`

  * `useScaleLerp`
    * If `true`, performs a scale lerp animation on the loading image.
    * Default: `true`
  * `lerpEndScale`
    * `Image.scale` ending factor for the scale lerp animation.
    * Default: `1.05`
  * `lerpDuration`
    * Duration for the scale lerp animation.
    * Default: `15`

`[LoadingTip.Text]`

  * `textPosition`
    * `LoadingTip.Text.position` value.
    * Default: `(0, 90)`
  * `textFontSize`
    * `LoadingTip.Text.fontSize` value.
    * Default: `24`
  * `textColor`
    * `LoadingTip.Text.color` value.
    * Default: `#FFFFFFFF`

`[LoadingScreen.Background]`

  * `backgroundColor`
    * Color to use for the loading screen background.
    * Default: `#000000FF`

`[LoadingScreen.PanelSeparator]`

  * `showPanelSeparator`
    * Show the panel separator image on the loading screen.
    * Default: `true`
  * `panelSeparatorPosition`
    * The position of the panel separator image on the loading screen.
    * Default: `(0, 150)`

</details>

## Notes

  * See source at: [GitHub/ComfyMods](https://github.com/redseiko/ComfyMods/tree/main/Intermission).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
