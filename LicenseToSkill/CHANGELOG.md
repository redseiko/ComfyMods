## Changelog

### 1.3.0

  * Fixed for the `v0.218.11` PTB patch.
  * Moved changelog to `CHANGELOG.md`.
  * Bumped up `<LangVersion>` to C# 10.
  * Code clean-up and refactoring.

### 1.2.1

  * Fixed bug where soft death status effect re-appeared on HUD after finishing it's countdown.
  * Fixed bug where hard death encountered after 10 minutes regardless of UI/mod enabled.
  * Updated assembly references to valheim_Data\Managed from unstripped_corlib.

### 1.2.0

  * Updated for `v0.216.5` PTB patch.
  * Converted `Skills.OnDeath()` prefix-patch to `Skills.LowerAllSkills()` prefix-patch.
  * Fixed `SEMan.AddStatusEffect()` prefix-patch to use `int nameHash` instead of `string name`.

### 1.1.1

  * Interim fix for `v0.212.6` Mistlands PTB.

### 1.1.0

  * Moved this mod from private repo to the public ComfyMods repo.
  * Moved all configuration code into new `PluginConfig` class.
  * Moved all Harmony-patching code into their own patch classes.
  * Added `manifest.json`, changed the `icon.png` and updated this `README.md`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.0.0

  * Initial release.