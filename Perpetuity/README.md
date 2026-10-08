# Perpetuity

*PersistentEventSystem control and customization.*

## Notes

  * Server-side only mod and should only be installed on dedicated servers.
  * Current overrides are hardcoded for Comfy, customization to be added in future updates. 

## Commands

  * `dump-persistent-event-data`

## Configuration

  * `[PersistentEventSystem]`
    * `isEnabled`
      * If false, active PersistentEvents are cleared and incoming RequestStartEvent RPCs are ignored.
    * `applyOverrides`
      * If true, applies overrides to PersistentEvents.
  * `[PersistentEvent]`
    * `centerPosition`
      * If true, PersistentEvent position will be overriden to the *center* of the sector.
    * `locationInstancesBlockPlacement`
      * If true, PersistentEvents will not spawn in sectors with an existing LocationInstance.
