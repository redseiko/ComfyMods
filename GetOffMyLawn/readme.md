# GetOffMyLawn

  * Set the health of any player building piece in the game to a configurable value.
    * The default value is set to 100,000,000.
  * You can also download the mod at:
    * Nexus Mods: https://www.nexusmods.com/valheim/mods/1349
    * Thunderstore: https://valheim.thunderstore.io/package/ComfyMods/GetOffMyLawn/

## User Interface

  * This fuctionality is available on repaired objects or placed build objects.
  * Set the health of the currently targeted build object to a high value by repairing it.
    * Note: build object must be either unwarded or on a ward the user can access).

## Instructions

  1. Extract `GetOffMyLawn.dll` to your `/Valheim/BepInEx/plugins/` folder.
  2. Place a new building piece or repair a building piece to set its health value to the configured value.
  3. Activate a ward to set the health value of all building pieces within range to the configured value.

## Changelog

### 1.3.0
  * Destroy Carts & Boats with the Hammer like regular build pieces

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