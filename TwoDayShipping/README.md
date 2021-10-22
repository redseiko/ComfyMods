# TwoDayShipping

  * Modifies the priority of several prefabs so that they are queued for sending soon than other prefabs.
    * For now this includes: guard_stone, guard_stone_test

## Details

  * This mod works on both clients and dedicated servers and rewrites the following methods:
    * `ZDOMan.ServerSortSendZDOS()`
    * `ZDOMan.ClientSortSendZDOS()`
    * `ZDOMan.ServerSendCompare()`
    * `ZDOMan.ClientSendCompare()`

  * Creates a static readonly `Comparison<ZDO>` instance for server and client compares instead of the vanilla
    behaviour of instantiating a new delegate for each method call.

  * This hooks into the following methods to check if the prefab being added or deserialized is one of the
    `guard_stone` prefabs and if so, changes the `ZDO.m_type` to `ZDO.ObjectType.Prioritized`.
    * `ZDOMan.AddToSector()`
    * `ZDOMan.RPC_ZDOData()`

## Changelog

### 1.0.0

  * Initial release.