## Changelog

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
