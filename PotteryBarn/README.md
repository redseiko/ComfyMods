# PotteryBarn

*Public build extension that adds existing game prefabs to the Hammer.*

## Installation

### Manual

  * Un-zip `PotteryBarn.dll` to your `/Valheim/BepInEx/plugins/` folder.

### Thunderstore (manual install)

  * Go to Settings > Import local mod > Select `PotteryBarn_v1.2.0.zip`.
  * Click "OK/Import local mod" on the pop-up for information.

## Instructions

  * New prefabs (that exist in the game) will be available to build!
  * You might need to pick-up/drop a stone/wood/resource to have the recipe trigger.

### Creator Shop

| Category    | Prefab                  | Crafting Station | Resource          | Resource           | Resource           | Resource     | Resource          |
|-------------|-------------------------|------------------|-------------------|--------------------|--------------------|--------------|-------------------|
| CreatorShop | goblin_banner           | Workbench        | 1x Blue mushroom  | 2x Finewood        | 2x Bone fragments  | 2x bloodbags | 6x Leather scraps |
| CreatorShop | goblin_fence            | Workbench        | 1x Blue mushroom  | 4x Wood            | 8x Bone fragments  |              |                   |
| CreatorShop | goblin_pole             | Workbench        | 1x Blue mushroom  | 2x Wood            | 4x Bone fragments  |              |                   |
| CreatorShop | goblin_pole_small       | Workbench        | 1x Blue mushroom  | 1x Wood            | 2x Bone fragments  |              |                   |
| CreatorShop | goblin_roof_45d         | Workbench        | 1x Blue mushroom  | 2x Wood            | 8x Bone fragments  | 2x Deer hide |                   |
| CreatorShop | goblin_roof_45d_corner  | Workbench        | 1x Blue mushroom  | 2x Wood            | 8x Bone fragments  | 2x Deer hide |                   |
| CreatorShop | goblin_roof_cap         | Workbench        | 4x Blue mushroom  | 10x Wood           | 12x Bone fragments | 6x Deer hide |                   |
| CreatorShop | goblin_stairs           | Workbench        | 1x Blue mushroom  | 2x Wood            | 4x Bone fragments  |              |                   |
| CreatorShop | goblin_stepladder       | Workbench        | 1x Blue mushroom  | 2x Wood            | 4x Bone fragments  |              |                   |
| CreatorShop | goblin_woodwall_1m      | Workbench        | 1x Blue mushroom  | 2x Wood            | 4x Bone fragments  |              |                   |
| CreatorShop | goblin_woodwall_2m      | Workbench        | 1x Blue mushroom  | 4x Wood            | 8x Bone fragments  |              |                   |
| CreatorShop | goblin_woodwall_2m_ribs | Workbench        | 1x Blue mushroom  | 4x Wood            | 8x Bone fragments  |              |                   |
| CreatorShop | GlowingMushroom         | Workbench        | 1x Blue mushroom  | 3x Yellow mushroom |                    |              |                   |
| CreatorShop | root07                  | Workbench        | 2x Ancient bark   |                    |                    |              |                   |
| CreatorShop | root08                  | Workbench        | 2x Ancient bark   |                    |                    |              |                   |
| CreatorShop | root11                  | Workbench        | 2x Ancient bark   |                    |                    |              |                   |
| CreatorShop | root12                  | Workbench        | 2x Ancient bark   |                    |                    |              |                   |
| CreatorShop | Skull1                  | Workbench        | 1x Blue mushroom  | 10x Bone fragments |                    |              |                   |
| CreatorShop | Skull2                  | Workbench        | 10x Blue mushroom | 50x Bone fragments |                    |              |                   |
| CreatorShop | StatueCorgi             | Stonecutter      | 5x Blue mushroom  | 20x Stone          |                    |              |                   |
| CreatorShop | StatueDeer              | Stonecutter      | 5x Blue mushroom  | 20x Stone          |                    |              |                   |
| CreatorShop | StatueEvil              | Stonecutter      | 5x Blue mushroom  | 20x Stone          |                    |              |                   |
| CreatorShop | StatueHare              | Stonecutter      | 5x Blue mushroom  | 20x Stone          |                    |              |                   |
| CreatorShop | StatueSeed              | Stonecutter      | 5x Blue mushroom  | 20x Stone          |                    |              |                   |
| CreatorShop | stonechest              | Stonecutter      | 10x Blue mushroom | 20x Stone          |                    |              |                   |
| CreatorShop | Vines                   | Workbench        | 2x Wood           |                    |                    |              |                   |

### Hammer

| Category  | Prefab            | Crafting Station | Resource     | Resource        | Resource |
|-----------|-------------------|------------------|--------------|-----------------|----------|
| Building  | Stone Floor 4x4   | Stonecutter      | 12x Stone    |                 |          |
| Building  | turf_roof         | Workbench        | 2x Wood      |                 |          |
| Building  | turf_roof_top     | Workbench        | 2x Wood      |                 |          |
| Building  | turf_roof_wall    | Workbench        | 2x Wood      |                 |          |
| Furniture | ArmorStand_Female | Workbench        | 8x Fine Wood | 2x Bronze Nails | 4x Tar   |
| Furniture | ArmorStand_Male   | Workbench        | 8x Fine Wood | 2x Bronze Nails | 4x Tar   |
| Furniture | wood_ledge        | Workbench        | 1x Wood      |                 |          |

### Changing the ArmorStand pose

  * The new ArmorStands have 15 different poses available!
  * There is a `Change Pose` switch at the base of the stand.
  * PotteryBarn fixes the error that prevents changing poses in vanilla.

### Notes

  * See source at: [GitHub](https://github.com/redseiko/ComfyMods/tree/main/PotteryBarn).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
  * Check out our community driven listing site at: [valheimlist.org](https://valheimlist.org/)

## Changelog

### 1.2.1

  * Actually include the updated `README.md` in the packaged file *sigh*.

### 1.2.0

  * Added more prefabs to the Hammer PieceTable.
  * Added `ArmorStand.SetPose()` prefix patch to eliminate NRE preventing pose changes.
  * Extracted patch-related code to new classes.
  * Extracted configuration-related code to `PluginConfig` class.

### 1.1.0

  * Added `manifest.json`, changed the `icon.png` and updated this `README.md`.
  * Modified the project file to automatically create a versioned Thunderstore package.

### 1.0.1

  * Reduced cost for Stone Floor 4x4 prefab from 24 stone to 12 stone.

### 1.0.0

  * Initial release.
  * Added Stone Floor 4x4 prefab to the Hammer's "Building" Options. 