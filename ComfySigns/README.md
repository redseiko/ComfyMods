# ComfySigns

*Colorful and comfy signs.*

## Features

### Sign text changes

  * Change default Sign text color to configured value (default: white).
  * Change default Sign text font to configured value (default: `Valheim-Norse`).
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
