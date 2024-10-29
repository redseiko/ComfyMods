## Changelog

### 1.5.0

  * Fixed for the `v0.219.13` patch.
  * Bumped up `<LangVersion>` to C# 12.
  * Code clean-up and refactoring.

### 1.4.0

  * Updated for `v0.217.46` patch.
  * Moved changelog into `CHANGELOG.md`.
  * Bumped up `<LangVersion>` to C# 10.
  * Added micro-optimizations to `RouteManager` related to `ZPackage` serialization.
  * Added new config option `nearbyRPCMethodNames`.

### 1.3.0

  * Updated for `v0.217.28` patch.
  * Updated project references from `unstripped_corlib` to `valheim_server_Data\Managed`.

### 1.2.0

  * Removed all client-side logic as `DestroyZDO` routing needs to be server-side due to stale ZDOs.
  * Removed `DestroyZDO` from nearby-routed methods in preparation for a different way of routing.
  * Re-write `ZDOMan.HandleDestroyedZDO()` as a prefix patch in preparation for routing to affected clients.
  * Added a `ZNet.UpdateNetTime()` postfix patch to cache the `ZNet.m_netTime` as long ticks value.

### 1.1.4

  * Fixed `DestroyZDO` RPCs never being routed because of a type where it was only checking `Destroy`.

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