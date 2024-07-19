## Changelog

### 1.14.0 (WIP)

  * Updated for the `v0.218.15` patch.
  * Bumped up `<LangVersion>` to C# 12.
  * Updated Jotunn dependency to `v2.20.1`.
  * New prefabs added, new "Builder Shop" tab, removed some existing prefabs, moved some prefabs to new tab.

### 1.13.0

  * Fixed for the `v0.217.46` patch.
  * Bumped up `<LangVersion>` to C# 10.
  * Updated Jotunn dependency to `v2.19.2`.
  * Code clean-up and refactoring.

### 1.12.0

  * Moved changelog to CHANGELOG.md
  * Added various prefabs.
  * Fixed bug where stone chest contents not dropped on destruction.

### 1.11.1

  * Removed prefagb dvergrprops_wood_stake due to missing mesh on broken status.

### 1.11.0

  * Updated for Jotunn dependency version update to `v2.13.0`.
  * Now hooks into `PieceManager.OnPiecesRegistered` event for adding pieces.

### 1.10.1

  * Removed demister ball since it is untargetable and cannot be deconstructed by players.

### 1.10.0

  * Fixed comfort on Female armor stand.

### 1.9.0

  * Added demisters to the CreatorShop.

### 1.8.0

  * Updated for `v0.217.14` patch.
  * Fixed the `Player.SetupPlacementGhost` transpiler patch and made it more robust for future patches.
  * Removed the `ArmorStand` patch as it's no longer needed.
  * Code clean-up and refactoring.
  * Created helper class `ReflectionUtils` and method `GetGenericMethod`.

### 1.7.0

  * Updated for `v0.216.9` patch.
  * Removed left-over logging statements (oops).
  * Code clean-up.

### 1.6.2

  * Fixed additional bug in drop behavior.

### 1.6.1

  * Fixed small bug in drop behavior.

### 1.6.0

  * Added dvergr pieces.
  * World generated pieces will have the same drop rate as vanilla if destroyed with damage or the build hammer.
  * Player constructed pieces will return recipe costs and may be broken by other players (not just the creator) for
    and items not on the CreatorShop tab.

### 1.5.1

  * Added an `EffectList.Create()` prefix patch to disable any enabled 'null' effectData.m_prefabs.
  * Updated mod icon to a new one created by [@jenniely](https://twitter.com/jenniely).

### 1.5.0

  * Moved 'Vines' and 'GlowingMushroom' to Cultivator under custom 'CreatorShop' category.
  * Added support for showing categories to Cultivator.
  * Code-clean up and refactoring.
  * Updated JVL dependency to `v2.10.4`.

### 1.4.0

  * Fixed for `v0.211.9` patch.
  * Added `BepInDependency` and `manifest.json` dependency to JVL.
  * Removed `yield return null` from `AddHammerPieces` coroutine.

### 1.3.0

  * CreatorShop changes?

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