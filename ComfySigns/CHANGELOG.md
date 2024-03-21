## Changelog

### 1.8.0

  * Fixed for the `v0.217.43` PTB patch.

### 1.7.0

  * Bumped up `<LangVersion>` to C# 10.
  * Moved changelog into `CHANGELOG.md`.
  * Migrated to new prototype `ComfyConfig` framework.
  * Removed `ExtendedColorConfigEntry` config drawer as functionality is now in `Configula` mod.
  * Added new config option:
    * `[Sign.Text.Render] maximumRenderDistance` - Maximum distance that signs can be from player to render sign text.

### 1.6.0

  * Updated for the `v0.217.38` patch.
  * Fixed sign text flickering at certain angles.

### 1.5.0

  * Fixed for the `v0.217.24` patch.

### 1.4.0

  * Updated for `v0.217.14` patch.
  * Modified handling the default `Valheim-Norse` font used for Signs to just change the `fontSharedMaterial`.
  * Renamed the config options for `defaultTextFont` and `defaultTextColor` to force new default handling above.
  * Now adds multiple fallback font assets for any selected font.

### 1.3.0

  * Added Norse SDF as global fallback font to support additional characters throughout font selections.

### 1.2.0

  * **Changed the PluginGuid from the template `comfy.valheim.modname` to `redseiko.valheim.comfysigns`.** (sigh)
  * Added SignEffectMaximumRenderDistance config option (default: 64m).
  * Added SignTextIgnoreSizeTags config option (default: false);
  * SignEffectEnablePartyEffect config option now defaults to false.
  * Added a one-second wait after a full party effect loop.

### 1.1.0

  * Added new Sign effect 'Party' feature.

### 1.0.0

  * Initial release.
