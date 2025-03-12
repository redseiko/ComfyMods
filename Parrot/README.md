# Parrot

*Polly want a cracker?*

## Features

### Add Server To PlayerList

  * Controlled by config-option `PlayerList.addServerToPlayerList` (default: `true`).
  * Adds a dummy `PlayerInfo` entry for the server to the list sent to clients in `SendPlayerList()`.
  * PlayerInfo entry for the server consists of:
    * Name: `Server`
    * CharacterId: `SessionId:4294967295` (`SessionId` is randomly assigned at start-up)
    * PlatformUserId: `Steam_90123456789012345` (randomly assigned at start-up)
  * When enabled, Parrot will drop all `Say` and `ChatMessage` RPCs that do not have a `targetPeerId == SessionId`.
    * Needed as clients now send the same RPC to *every single other peer* and we want to prevent duplicate messages.
    * The RPC targeting the server will then have its `targetPeerId` set to 0 to broadcast to all.