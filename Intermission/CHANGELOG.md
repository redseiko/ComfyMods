## Changelog

### Unreleased

  * Display a black background behind custom loading images for a consistent experience no matter the image or screen aspect ratio.

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