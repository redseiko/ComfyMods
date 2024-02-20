## Changelog

### 2.5.0

  * Bumped up `<LangVersion>` to C# 10.
  * Added new config option `filterInWorldShoutText` (default: false) that will also apply shout filters to in-world
    shout message texts. 
  * Fixed a bug where the `ChatPanel` would pop-up/display if the message was filtered.

### 2.4.0

  * Added compatibility with [Groups](https://valheim.thunderstore.io/package/Smoothbrain/Groups/).

### 2.3.0

  * Fixed for the `v0.217.38` patch.

### 2.2.1

  * Fixed a bug where default chat modes could not be toggled properly via chat command.

### 2.2.0

  * Fixed for the `v0.217.22` patch.

### 2.1.0

  * Fixed a bug where message dividers were not hidden when the message type toggle was off.
  * Made the MessageType toggles use the Valheim button sprite.
  * Added the panel resizer button with new button style.

### 2.0.0

  * Split off Changelog section into a new `CHANGELOG.md` file.
  * Rewrote entire mod, top-down to be less hacky and use TMP entirely.
  * Some features are still missing in the rewrite, to be added later.

### 1.5.0

  * Updated almost all `Text` components to `TMP_Text` equivalents.
  * Added a work-around for OS-installed fonts as TMP requires the original font file (and can't use a dynamic font).
  * Modified resizer icon and message toggle styles.
  * Added config options for message toggles `Style.MessageToggle.Text`.

### 1.4.1

  * Applied the 'Outline' font material to in-world texts.
  * Set a plain sprite for all Image components in the ChatPanel.

### 1.4.0

  * Updated for the `v0.214.2` patch.
  * ChatPanel now hides when the Hud is hidden.
  * Updated mod icon to a new one created by [@jenniely](https://twitter.com/jenniely).

### 1.3.0

  * Modified `ShouldCreateDivider()` to also check if the username is different.
  * Added new feature to change the default message type from [say] to one of [say/shout/whisper] via chat command.
  * Added new config option 'chatPanelContentRowTogglesToEnable' to use for enabling/disable the toggles at start.
  * Added new config option 'chatPanelDefaultMessageTypeToUse' to use for initial default chat message type at start.
  * Added `CachedValues` to `StringListConfigEntry` and use that for message filtering.
  * Some code-refactoring and organizing.

### 1.2.1

  * Fixed a bug where if no filters are defined it filters **everything** (sadface).

### 1.2.0

  * Added support for chat filters configurable using a custom drawer in ConfigurationManager.

### 1.1.0

  * Added support for a new chat message layout `SingleRow`.
  * Added new configuration options for timestamp and content spacing.
  * Refactored code to unify UI creation from all message types and rebuild from message history.

### 1.0.0

  * Initial release.