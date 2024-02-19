## Changelog

### 1.4.0

  * Bumped up `<LangVersion>` to C# 10.
  * Created new `RequestManager` to manage pending teleport requests including loading/saving to file.
  * Added config option `requestListFilename` to specify the file to load/save to.

### 1.3.0

  * Added a second RPC `RequestTeleportByZDOID` to handle a teleport player request using a player's ZDOID.

### 1.2.1

  * Fix missing Harmony annotations on `RPC_PeerInfoTranspiler`.

### 1.2.0

  * Added new RPC `RequestTeleport` to enable client-side teleport player requests with access control and logging.

### 1.1.0

  * Added additional logging to TeleportManager.

### 1.0.0

  * Initial release.