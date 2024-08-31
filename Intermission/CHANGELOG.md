## Changelog

### 1.7.0

  * Updated for the `v0.218.21` patch.
  * Fixed the `LoadingIndicator` not centering on the initial `SceneLoader` screen.
  * Added a new config option `SceneLoadeer.progressIndicatorOffset` for the centered indicator.

### 1.6.0

  * Updated for the `v0.218.19` patch.
  * Added a solid color background to the loading screen to handle images that do not fill the entire screen.
    * Color can be changed with config option: `LoadingScreen.Background.backgroundColor`
  * Small code clean-up and refactornig.

### 1.5.0

  * Updated for the `v0.217.46` patch.
  * Added new `SceneLoader` patch for custom loading images and progress indicator.
  * Added new config options to control new feature:
    * `SceneLoader.useLoadingImages`
    * `SceneLoader.showProgressText`
    * `SceneLoader.centerProgressIndicator`

### 1.4.0

  * Updated for the `v0.217.38` patch.
  * Moved changelog into `CHANGELOG.md`.
  * Bumped up `<LangVersion>` to C# 10.
  * Custom loading images will now be randomly shown at least once before resorting into a new random order.
  * Code clean-up and refactoring.

### 1.3.0

  * Fixed for the `v0.217.24` patch.
  * Added work-around for the `FejdStartup` loading text UI state being locked due to the `menuAnimator`.
  * Removed `shadowEffectColor` and `shadowEffectDistance` config options due to the change to `TextMeshPro`.

### 1.2.0

  * Added support for `.jpg` image files.

### 1.1.0

  * Updated for Valheim `v0.216.9` patch.

### 1.0.2

  * Modified TipText to horizontally wrap with width set to entire screen.
  * Updated BepInEx dependency to `denikson-BepInExPack_Valheim-5.4.2100`.

### 1.0.1

  * Fixed a bug where config changes to LoadingImage and PanelSeparator were not reflected accurately.

### 1.0.0

  * Initial release.
