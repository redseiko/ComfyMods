# Unbuildable

*Server-side ZDO placement control and customization.*

## Features

  * Removes ZDOs matching specified prefabs with `WearNTear` using `RPC_Remove` on ZDO creation.
  * Destroys ZDOs matching specified prefabs on ZDO creation.

## Configuration

`[Prefabs]`

  * `prefabsToRemoveList`
    * Comma-separated list of prefabs (with `WearNTear`) to remove via `RPC_Remove` on ZDO creation.
    * Default: `(empty string)`
  * `prefabsToDestroyList`
    * Comma-separated list of prefabs to destroy directly on ZDO creation.
    * Default: `(empty string)`

## Notes

  * See source at: [GitHub/ComfyMods](https://github.com/redseiko/ComfyMods/tree/main/Unbuildable).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
