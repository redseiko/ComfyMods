# GetOffMyLawn

  * Set the health of player-placed items in the game to a configurable value.
  * Reduces monster attacks on player objects.

## Installation

### Manual

  * Un-zip `GetOffMyLawn.dll` to your `/Valheim/BepInEx/plugins/` folder.

### Thunderstore (manual install)

  * Go to Settings > Import local mod > Select `GetOffMyLawn_v1.4.0.zip`.
  * Click "OK/Import local mod" on the pop-up for information.

## Instructions

### User Interface

  * Fuctionality is available on repaired objects or placed build objects.

  - Set the health of the currently targeted build object to a high value by repairing it.
    - Note: build object must be either unwarded or on a ward the user can access.

### Usage

  * Health will be set when you place the item or when you use the repair hammer on it.
    * You can only change the health of building pieces that you own or are permitted on.

  - You can activate a ward to set the health value of all building pieces within range to the configured value.

  * Great for boats!
    * There are often "lag" issues with the game that cause boats to take more damage than they're supposed to.
    * Just set your boat to high health. 

  - All items placed with the hammer have their health changed.
    - If you're using a mod that spawns things with the hammer, the ore will likely have high health.
    - Simply disable GOML in the configuration manager if you're trying to place normal health ore veins for example.

  * **Disabling the mod does not change the health of previously placed/repaired pieces.**
    * To lower the health again you'll need set a low health value and repair the piece or activate a ward in radius.

### Ignoring Piece Stability

  * You can use this mod to ignore piece stability by setting piece health to a very high value (`1E+17` or higher).

  - **Caution!**
    - Stability in the game helps keep buildings smaller to reduce lag.
    - When you cheat stability you can create larger buildings but this causes more lag due to more instances.
    - Keep an eye on your instances with `F2` panel if this is a concern for you.

  * Why this works...
    * Pieces with zero stability incur piece health damage over time (100% vanilla base health / second).
    * With high enough health it just takes such a long time to tick down (years) it doesn't matter.

  - **Every piece health damage message is broadcasted to everyone on the server.**
    * The setting `EnablePieceHealthDamageThreshold` will restrict this behaviour to only pieces with < 100K health.

### Recommended Mods to Use

  * [ConfigurationManager](https://valheim.thunderstore.io/package/Azumatt/Official_BepInEx_ConfigurationManager/)﻿.
    * Press F1 and navigate to the GetOffMyLawn section to change the health value.
  * [EulersRuler](https://valheim.thunderstore.io/package/ComfyMods/EulersRuler/)﻿
    * See piece health, stability and other information while building.

### Notes

  * See source at: [GitHub](https://github.com/redseiko/ComfyMods/tree/main/GetOffMyLawn).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
  * Check out our community driven listing site at: [valheimlist.org](https://valheimlist.org/)

## Changelog

### 1.4.1

  * Repairs with negative damage should now take effect. Vanilla repair previously overwrote negative repair values.

### 1.4.0

  * Moved all configuration code into new `PluginConfig` class.
  * Moved all Harmony-patching code into their own patch classes.
  * **Increased the default `PieceHealth` value to `1E+17`.**
  * Added `manifest.json` and updated this `README.md`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.3.1

  * Destroy carts & boats with the Hammer like regular build pieces.

### 1.2.1

  * Actually check the `enablePieceHealthDamageThreshold` config value for the WearNTear.ApplyDamage() patch.

### 1.2.0

  * Added new optimization/configuration option `enablePieceHealthDamageThreshold`.
    * Pieces with health that exceed 100K **will not** execute `WearNTear.ApplyDamage()` meaning they will not
      take any piece damage. Subsequently, they **will not** send a `WNTHealthChanged` message to the server.
    * This reduces the overall send and receive rates for every player on the server as they will no longer receive
      the message used only for syncing the visual condition of pieces across clients.

### 1.0.1

  * Added null-checks for Piece and Piece.ZNetView references in the ward-interaction method.

### 1.0.0

  * Updated for Hearth & Home.