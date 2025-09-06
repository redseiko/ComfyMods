## Changelog

### 1.2.0

  * Bumped up `<LangVersion>` to C# 12.
  * Added new alternate `DrawTooltip()` functionality and config-options:
    * `[Tooltip] useAlternateDrawTooltip`
    * `[Tooltip] alternateTooltipWidth`
  * Added new `OnGUI` prefix-patch to clear `GUI.tooltip` to fix a Unity-v6 regression.

### 1.1.0

  * Bumped up `<LangVersion>` to C# 10.
  * Moved changelog into `CHANGELOG.md`.
  * Added new `Vector4SettingField` and `QuaternionSettingField`.
  * Reworked `FloatSettingField` to use the common `FloatInputField`.
  * Updated mod icon to a new one created by [@jenniely](https://twitter.com/jenniely).

### 1.0.0

  * Initial release.
