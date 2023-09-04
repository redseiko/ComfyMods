# ComfySigns

*Colorful and comfy signs.*

## Features

### Sign text changes

  * Change default Sign text color to configured value (default: white).
  * Change default Sign text font to configured value (default: Norse SDF).
  * Change Sign text character limit from 50 to 999.
  * Config option to ignore <size> tags in Sign text.

### Sign TextInput panel changes

  * InputField set to plain formatted text to show rich text tag editing.
  * Panel can be moved around with mouse-click and drag action.

### Logging changes

  * Suppress "Unicode value not found" log warnings.

## Sign Effects

  * All sign effects will only render when the sign is within `64m` (configurable) of your player.

### Party Mode

  * The `enablePartyEffect` config option **must be turned on** (in `SignEffect.Party` section).
  * Wrap your sign text with this tag: `<link=party>Sign Text Goes Here</link>`

## Changelog

### 1.4.0

  * Updated for `v0.217.14` patch.
  * Modified handling the default `Valheim-Norse` font used for Signs to just change the `fontSharedMaterial`.
  * Renamed the config options for `defaultTextFont` and `defaultTextColor` to force new default handling above.


### 1.3.0

  * Added Norse SDF as global fallback font to support additional characters throughout font selections.

### 1.2.0

  * **Changed the PluginGuid from the template `comfy.valheim.modname` to `redseiko.valheim.comfysigns`.** (sigh)
  * Added SignEffectMaximumRenderDistance config option (default: 64m).
  * Added SignTextIgnoreSizeTags config option (default: false);
  * SignEffectEnablePartyEffect config option now defaults to false.
  * Added a one-second wait after a full party effect loop.

### 1.1.0

  * Added new Sign effect 'Party' feature.

### 1.0.0

  * Initial release.