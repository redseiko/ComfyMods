# SkyTree

*Makes the tree in the sky (Yggdrasil) solid and a foundation to build on.*

## Features

  * Sets the layer for the `YggdrasilBranch` prefab and `branch` child from 19 (`skybox`) to 15 (`static_solid`).
  * Adds a `MeshCollider` component to the `YggdrasilBranch/branch` child.
  * Sets `RenderGroupSubscriber.Group` on `YggdrasilBranch/branch` to `Always`.

## Limitations

  * Valheim considers *any* position with `height/y >= 3000m` as being in a dungeon which means:
    * Most build pieces cannot be placed without the `DungeonBuild` world modifier enabled.
    * Overworld terrain and grass below stop rendering.
    * Outdoor-only weapons and `RandEventSystem` raids are disabled.

## Notes

  * See source at: [GitHub/ComfyMods](https://github.com/redseiko/ComfyMods/tree/main/SkyTree).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
