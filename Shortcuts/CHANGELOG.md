## Changelog

### 1.8.0

  * Fixed for the `v0.221.4` patch.
  * Created `ZInput.Constructor` postfix-patch as entry-point to bind shortcut-configs.

### 1.7.0

  * Fixed for the `v0.218.14` patch.
  * Bumped up `LangVersion` to C# 12.

### 1.6.0

  * Fixed for `v0.217.43` PTB patch.
  * Moved changelog into `CHANGELOG.md`.
  * Bumped up `LangVersion` to C# 10.

### 1.5.0

  * Updated for `v0.217.37` PTB.
  * Created a new `ShortcutConfigEntry` wrapper to use with rewritten transpiler logic for improved efficiency.

### 1.4.0

  * Updated code match references for Input class to ZInput class for compatibility with patch 0.217.14.

### 1.3.0

  * Updated for `v0.214.2` PTB.
  * Replaced the special `IsDown()` with a simpler method that uses `Input.GetKey()/GetKeyDown()`.

### 1.2.0

  * Prototype using a special version of `IsDown` modified from BepInEx's KeyboardShortcut code.
  * Clean-up some of the transpiler delegate code and the PluginConfig code.
  * Added `manifest.json`, `icon.png` and `README.md`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.1.0

  * Updated for Hearth and Home.
  * Added toggles for:
    * Debugmode - removedrops `L`
    * ConnectPanel - toggle `F2`

### 1.0.0

  * Initial release.
