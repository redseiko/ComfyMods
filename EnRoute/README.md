# EnRoute

*ZRoutedRPC tweaks and optimization.*

## Changelog

### 1.1.3

  * Change bounding-box check to bounding-box intersection check to really matchin vanilla behaviour.

### 1.1.2

  * Change sector range checks from radius to bounding-box to match vanilla `ZDOMan` behaviour.

### 1.1.1

  * Hot-fix for server not sending LocationIcons as RouteRecords were created after AddPeer instead of before it.

### 1.1.0

  * Added server-side routing logic.

### 1.0.1

  * Fixed a critical bug by always routing the RPC to the server peer first.

### 1.0.0

  * Initial release.