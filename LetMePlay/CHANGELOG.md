## Changelog

### 1.5.0
  * Updated `Ashrain` to `Ashlands_ashrain`
  * Added `Ashlands_Storm`
  * Moved the Changelog out of README and into a separate CHANGELOG file
  * Bumped <LangVersion> to C# 12
  * Bumped BepIn Dependancy to `5.4.2202`
  * Recompiled against game version `0.218.19`

### 1.4.0

  * Moved all configuration code into new `PluginConfig` class.
  * Moved all Harmony-patching code into their own patch classes.

  - Added `manifest.json`, changed the `icon.png` and updated this `README.md`.
  - Modified the project file to automatically create a versioned Thunderstore package.

### 1.3.0

  * Added patch to `SpawnArea.Awake()` where it will check if there are any null prefabs in its `m_prefabs` list and if
    so, remove them.

### 1.2.0

  * Added two toggles `disableWeatherSnowParticles` and `disableWeatherAshParticles`.
    * These disable the snow particles during Snow/SnowStorm weather and the ash particles during Ashrain weather.

### 1.1.0

  * Updated for Hearth and Home.

### 1.0.0

  * Initial release after updating to HarmonyX.