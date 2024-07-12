# LetMePlay

*Collection of tweaks and mods to improve player experience and accessibility.*

## Installation

### Manual

  * Un-zip `LetMePlay.dll` to your `/Valheim/BepInEx/plugins/` folder.

### Thunderstore (manual install)

  * Go to Settings > Import local mod > Select `LetMePlay_v1.5.0.zip`.
  * Click "OK/Import local mod" on the pop-up for information.

## Instructions

  * Most toggles/options are **initially disabled**, there are two ways to toggle them.

  - In-game ConfigurationManager
    - Press F1 in-game to open the ConfigurationManager and navigate to "Let Me Play" section.

  * Edit the BepInEx configuration file
    * Start the game once to generate the configuration file.
    * Navigate to and open: `<install folder>\Valheim\BepInEx\config\redseiko.valheim.letmeplay.cfg`

## Features

### Wards

  * Disable wards from flashing their blue shield.

### Camera

  * Disable camera sway (which is due to player head animation) when sitting on a chair/bench.

### Inventory

  * Fixes interaction with non-player items (GoblinSpear, etc) in player inventory and chests.
    * These items will have the `grey hammer` icon and the prefab name.
    * These items will have a special description and this mod as the crafter name.

### Build

  * Disables the yellow placement indicator when building.
    * If you are using gizmo, the gizmo indicator will also be disabled.

### Weather

  * Disable snow particles during Snow/SnowStorm weather.
  * Disable ash particles during Ashrain weather.

### SpawnArea

  * Removes any null prefabs in in `SpawnArea.m_prefabs` on `SpawnArea.Awake()`.

### Game

  * Disable the Game's 'pause' effect when the Menu is shown.