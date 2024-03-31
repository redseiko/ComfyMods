# GetOffMyLawn

  * Set the health of player-placed items in the game to a configurable value.
  * Reduces monster attacks on player objects.

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

  * [ConfigurationManager](https://thunderstore.io/c/valheim/p/Azumatt/Official_BepInEx_ConfigurationManager/)﻿.
    * Press F1 and navigate to the GetOffMyLawn section to change the health value.
  * [Configula](https://thunderstore.io/c/valheim/p/ComfyMods/Configula/)
    * Modifies the ConfigurationManager for more user-friendly input.
  * [EulersRuler](https://thunderstore.io/c/valheim/p/ComfyMods/EulersRuler/)﻿
    * See piece health, stability and other information while building.

### Notes

  * See source at: [GitHub/ComfyMods](https://github.com/redseiko/ComfyMods/tree/main/GetOffMyLawn).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
