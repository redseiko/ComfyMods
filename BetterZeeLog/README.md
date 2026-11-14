# BetterZeeLog

*Logging zees the better way.*

## Features

  * Modifies all `ZLog.Log*` methods to prefix a timestamp and remove the trailing new-line.
  * Removes stack traces for `Info` and `Warning` log types.
  * Removes `Failed to send data` logging to reduce log spam.
  * Removes `Activating default/right-stick element` logging to reduce log spam.
  * Removes `Players wants to open/but im not the owner` logging to reduce log spam.
  * Removes `Missing prefab hash: ` logging to reduce log spam.
  * Removes `placed at height`, `placed at world height` and `Placed ...` logging to reduce log spam.
  * Removes `Missing item prefab` and `Failed to get attach prefab for item` logging to reduce log spam.

## Configuration

All config-option changes require a full game restart to take effect.

`[_Global]`

  * `isModEnabled`
    * Globally enable or disable this mod (restart required).
    * Default: `true`

`[Logging]`

  * `removeStackTraceForNonErrorLogType`
    * Disables the stack trace for 'Info' and 'Warning' log types.
    * Default: `true`
  * `removeFailedToSendDataLogging`
    * Removes 'Failed to send data' logging in `ZSteamSocket.SendQueuedPackages()`.
    * Default: `true`
  * `removeContainerRequestOpenLogging`
    * Removes (NOPs out) 'Players wants to open/but im not the owner' logging in `Container.RPC_RequestOpen()`.
    * Default: `true`
  * `removePlayerPlacePieceLogging`
    * Removes 'placed at height'/'placed at world height' logging in `Player.PlacePiece()`.
    * Default: `true`
  * `removePlayerTryPlacePieceLogging`
    * Removes 'Placed' logging in `Player.TryPlacePiece()`.
    * Default: `true`
  * `removeArmorStandSetVisualItemLogging`
    * Removes 'Missing item prefab' logging in `ArmorStand.SetVisualItem()`.
    * Default: `true`
  * `removeItemStandSetVisualItemLogging`
    * Removes 'Missing item prefab' and 'Failed to get attach prefab for item' logging in `ItemStand.SetVisualItem()`.
    * Default: `true`
  * `removePickableItemSetupItemLogging`
    * Removes 'Failed to get attach prefab for item' logging in `PickableItem.SetupItem()`.
    * Default: `true`

`[Quaternion.LookRotation]`

  * `checkProjectFixedUpdatedZeroVelocity`
    * Checks for zero `m_vel` in `Projectile.FixedUpdate()`.
    * Default: `true`

## Notes

  * See source at: [GitHub/ComfyMods](https://github.com/redseiko/ComfyMods/tree/main/BetterZeeLog).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
  * BetterZeeLog icon created by [@jenniely](https://twitter.com/jenniely) ([jenniely.com](https://jenniely.com)).
