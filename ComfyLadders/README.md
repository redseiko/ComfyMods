# ComfyLadders

*Ladders made comfy.*

## Features

ComfyLadders is a *replacement* for [BetterLadders](https://thunderstore.io/c/valheim/p/Amar1729/BetterLadders/) that
adds configuration-options to toggle functionality in-game.

  * Global toggle:
    * `isModEnabled` (default: `true`)
  * Individual toggles for each ladder prefab:
    * `ashlandSteepstairIsEligible` (default: `true`)
    * `goblinStepladderIsEligible` (default: `false`)
    * `graustenStoneLadderIsEligible` (default: `true`)
    * `woodStepladderIsEligible` (default: `true`)

## Custom-Fields

There is support for ZDO custom-fields to force vanilla functionality for any ladder ZDO.

  * Component: `AutoJumpLedge`
  * Field: `m_isEligibleOverride`
  * Value: `1`

## Compatibility

ComfyLadders will *unpatch* BetterLadders if both mods are installed.

## Notes

  * See source at: [GitHub/ComfyMods](https://github.com/redseiko/ComfyMods/tree/main/ComfyLadders).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
