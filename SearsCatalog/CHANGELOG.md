## Changelog

### 1.8.0

  * Modified `Categories` and `TabBorder` to now take up the full-space of the `BuildHud.Panel`.
  * Disabled `InputHelp` (key-hints) to make room for expanded `Categories`.
  * Added logic to trigger Jotunn category-rebuild whenever the `BuildHud.Panel` is resized or on config-change.
  * Added new config-options:
    * `[BuildHud.Panel.Categories] categoriesRectPosition`
    * `[BuildHud.Panel.Categories] categoriesRectSizeDelta`
    * `[BuildHud.Panel.TabBorder] tabBorderIsEnabled`
    * `[BuildHud.Panel.TabBorder] tabBorderRectPosition`
    * `[BuildHud.Panel.TabBorder] tabBorderSizeDelta`
    * `[BuildHud.Panel.InputHelp] inputHelpIsEnabled`
    * `[BuildHud.Panel.InputHelp] inputHelpRectPosition`
    * `[BuildHud.Panel.InputHelp] inputHelpRectSizeDelta`
  * Minor code clean-up.

### 1.7.0

  * Fixed for the `v0.221.4` patch.
  * Fixed scroll-sensitivity issue with the build-panel scrollbar.
  * Added new config-option `[BuildHud.Panel] buildHudPanelScrollSensitivity` to customize scroll-sensitivity.
  * Added an invisible image to the `Hud.m_pieceListRoot` to fix `ScrollRect.content` raycast-hit detection.
  * Minor code clean-up.

### 1.6.0

  * Fixed for the `v0.219.10` PTB patch.

### 1.5.1

  * Added missing negative index check in `CenterOnSelectedIndex()`.

### 1.5.0

  * Fixed for the `v0.218.16` patch.
  * Moved changelog into `CHANGELOG.md`.
  * Bumped up `<LangVersion>` to C# 12.
  * Code clean-up and refactoring.
  * Removed several config options as the related code is no longer needed.

### 1.4.0

  * Updated for `v0.217.31` patch.
  * Fixed a bug with mouse scroll-wheel scrolling through category tags instead of the build panel pieces.
  * Fixed a bug with selecting the correct build piece when using the vanilla 'copy piece' function.
  * Removed logic related to un-patching deprecated BuildExpansion if detected.
  * Minor code clean-up.

### 1.3.0

  * Updated for `v0.217.14` patch.

### 1.2.0

  * Updated for `v0.214.2` PTB.
  * Updated mod icon to a new one created by [@jenniely](https://twitter.com/jenniely).

### 1.1.0

  * Added `PieceTable.Left/Right/Up/DownPiece()* patches for controller support.
  * Added `PanelResizer` and a resizer icon to the build panel.

### 1.0.0

  * Initial release.
