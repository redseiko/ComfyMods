# Hashiko

*Loyal preloader-patcher for GetStableHashCode.*

## Features

  * Checks if `StringExtensionMethods.GetStableHashCode()` exists in `assembly_utils.dll` and if not, adds it back in.

## Logging

If method exists:

    [Info   :   BepInEx] Patching [assembly_utils] with [Hashiko.Hashiko]
    [Info   :   Hashiko] Found method `GetStableHashCode(this string str)`.

If method does not exist:

    [Info   :   BepInEx] Patching [assembly_utils] with [Hashiko.Hashiko]
    [Info   :   Hashiko] Method `GetStableHashCode(this string str)` not found. Adding...
    [Info   :   Hashiko] Successfully added method `GetStableHashCode(this string str)`.

## Notes

  * See source at: [GitHub/ComfyMods](https://github.com/redseiko/ComfyMods/tree/main/Hashiko).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
