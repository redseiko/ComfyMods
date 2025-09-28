# StatusQuo

*Compact StatusEffect list display.*

![Splash](https://imgur.com/INWeOzh.png)

## Features

  * Replaces the vanilla horizontal StatusEffect list display with a compact vertical list.
  * Shows multiple columns when there are many StatusEffects, can be configured (see below).
  * Can move the StatusEffect list position to be below the Minimap (see below).

## Configuration

All configuration options can be modified in-game and will take effect immediately.

![Configuration](https://imgur.com/xGPg6Ov.png)

### Position Below Minimap

  * Set `[StatusEffectList] rectPosition` to `X: -40`, `Y: -250`.
  * Set `[StatusEffect] maxRows` to `0` for unlimited rows.
  * Adjust `[StatusEffect] rectSizeDelta`: `X` is width, `Y` is height

![Vertical](https://imgur.com/kYOWAgw.png)

## Compatibility

### [MinimalStatusEffects](https://thunderstore.io/c/valheim/p/RandyKnapp/MinimalStatusEffects/)

StatusQuo is a *replacement* for MinimalStatusEffects and will show an *incompatibility* error if both are loaded.

    Could not load [StatusQuo 1.0.0] because it is incompatible with: randyknapp.mods.minimalstatuseffects

## Notes

  * This is the *good enough* release. More features and polish to be added later.
  * See source at: [GitHub/ComfyMods](https://github.com/redseiko/ComfyMods/tree/main/StatusQuo).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
