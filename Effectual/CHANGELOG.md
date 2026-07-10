## Changelog

### 1.1.0

  * Updated for the `v0.221.12` patch.
  * Migrated to SDK-style project.
  * Added new `EffectArea.GetBaseValue()` transpiler patch to also check and increase `m_tempColliders` buffer size.
  * Added new `EffectArea.CustomFixedUpdate()` transpiler patch to check for valid `Character.m_nview`.
  * Added new `Character.OnDestroy()` transpiler patch to remove destroyed `Character` reference from all
    `EffectArea.m_collidedWithCharacter` lists.

### 1.0.0

  * Initial release.
