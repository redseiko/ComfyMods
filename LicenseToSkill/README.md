# LicenseToSkill

*Lose less skill on death and increase skill loss prevention timer.*

## Instructions

### Features

  * Increases 'no skill loss' status effect from 10 minutes to 20 minutes.
    * Now shows the current time remaining!
  * Reduces skill loss on death from 5% across all skills to 1%.

 ## Installation

### Manual

  * Un-zip `LicenseToSkill.dll` to your `/Valheim/BepInEx/plugins/` folder.

### Thunderstore (manual install)

  * Go to Settings > Import local mod > Select `LicenseToSkill_v1.2.1.zip`.
  * Click "OK/Import local mod" on the pop-up for information.

### Notes

  * See source at: [GitHub](https://github.com/redseiko/ComfyMods/tree/main/LicenseToSkill).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
  * Check out our community driven listing site at: [valheimlist.org](https://valheimlist.org/)

## Changelog

### 1.2.1

  * Fixed bug where soft death status effect re-appeared on HUD after finishing it's countdown.
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