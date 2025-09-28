## Changelog

### 1.6.0

  * Updated for the `v0.221.4` patch.

### 1.5.1

  * Hot-fix to handle missed case that `Chat.SendPing()` remains on original pre-patch logic.

### 1.5.0

  * Updated for the `v0.220.3` patch.
  * Removed `suppressSayGamertag` and `suppressChatMessageGamertag` feature as this no longer happens in vanilla.
  * Added new logic to handle annoying `Say`/`ChatMessage` RPC logic in clients.
  * Added new config-option `addServerToPlayerList` to add a PlayerInfo entry for the server.
  * Added new config-option `allowParrotServerConnections` for experimental WIP server-to-server messaging.

### 1.4.0

  * Updated for the `v0.218.12` PTB patch and `BetterZeeRouter v1.9.0` update.

### 1.3.0

  * Fixed for the `v0.217.46` patch.
  * Removed previous functionality for now to be re-implemented later.
  * Added new functionality to suppress `UserInfo.Gamertag` from all `ChatMessage` and `Say` RPCs.

### 1.2.0

  * Add dependency to BetterZeeRouter mod and create new SayHandler and ChatMessageHandler.
  * Add a 250ms delay in `DiscordWebhookClient.UploadLoopAsync()` to avoid overrunning Discord API rate limiting.

### 1.1.1

  * Add `.ConfigureAwait(false)` to the `UploadLoop()` so that it does not block the main thread.

### 1.1.0

  * Created AsyncQueue and DiscordUploadClient for fully asynchronous uploading to Discord Webhooks.
  * Added option for a Shout-only Discord Webhook for public channels.
 
### 1.0.0

  * Initial release.
