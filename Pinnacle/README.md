# Pinnacle

*Pinnacle perpetually provides premium pin performance.*

![Pinnacle - At a Glance](https://imgur.com/Wabfnru.png)

## New Features

### Quick Map Pins

  * When the ***Minimap is open*** you can now use a keyboard-shortcut to create a pin at your current position.
  * See the `QuickMapPin` config-options to set the shortcut and default pin-name and pin-type.

### Command: `add-map-pin`

    add-map-pin [--position=<x,y,z>] [--pin-name=<string>] [--pin-type=<pin-type>] [--log]
    add-map-pin [--pos=<x,y,z>] [--name=<string>] [--type=<pin-type>] [--log]

    <pin-type> must be one of the following values:

      Icon0    None      Shout          Hildir1
      Icon1    Death     Ping           Hildir2
      Icon2    Bed       RandomEvent    Hildir3
      Icon3    Player    EventArea
      Icon4    Boss

  * Adds a new Minimap pin using the specified position, pin-name and pin-type.
  * If `--position` **is not specified**, position will be set to your current player position.
  * If `--pin-name` **is not specified**, pin-name will be set to empty.
  * If `--pin-type` **is not specified**, pin-type will be set to `Icon3`.
  * If `--log` **is specified**, confirmation message will be sent to chat-window and console.

### Pin Icon Tags

  * You can now use tags at the end of the pin name to customize the pin icon!

    ![Pinnacle - Icon Tags](https://imgur.com/lMivcpW.png)

  - Icon customizations will be visible to those with Pinnacle enabled.
  - Vanilla players will see the vanilla pin icon without customization.
  - Tags will be stripped from the displayed pin name underneath the icon.

#### Colorizing Icon

  * Add a hex-code **to the end** of a pin-name in the format of: `[#F9F9F9]`
    * For example: `Home Base [#ff0000]`
    * The hex-code must start with `#`, then exactly 6 hexadecimal characters (lower-case or upper-case).
  * Colorizing icons changes the icon sprite's *shader color* from the default `#ffffff` value.
    * Colorizing non black-white sprites may not work as intended.

#### Scaling Icon Size

  * Add a percentage **to the end** of a pin-name in the format of : `[150%]` (note the percent at the end)
    * For example: `Home Base [80%]`
    * Only two and three-digit values are accepted
    * All values are *clamped* to a min/max range of 50% to 200%.

#### Changing Icon Sprite

  * Add a sprite-name **to the end** of a pin-name in the format of: `[:sprite_name]` (note the colon at the front)
    * For example: `Home Base [:egg]`
    * The full list of sprite names are here: <https://valheim-modding.github.io/Jotunn/data/gui/sprite-list.html>
    * The sprite name must **match exactly** (case-sensitive) and be at least 3 characters long.

#### Combining Icon Tags

  * You can combine all tags in any order as long as the pin name ends with a `]`.
    * For example: `Home Base [:egg] [#00ff00] [125%]`
    * Another example: `Away Base: [#990099] [80%]`

#### Tags Configuration

  * Config-options for icon tags are under the `Pinicon.Tags` section.

## Features

### PinEditPanel

  * Edit existing pins and add new pins with more icon types!

    ![Pinnacle - Edit an Existing Pin](https://imgur.com/ODB2jVz.png)

  - Edit an existing pin
    - Left-click on an existing pin on the map.
    - The *PinEditPanel* will toggle on with the default pin data.
    - You can modify the pin name, icon and position.
    - You can toggle the pin checked state and shared state.

  * Add a new pin
    * Left-double-click on the target point on the map to add a new pin.
    * The *PinEditPanel* will toggle on with default pin data.

  - Click anywhere on the map without a pin to toggle off the *PinEditPanel*.

### PinListPanel

  * Lists all your pins or filter them by pin name!

    ![Pinnacle - Show the PinListPanel](https://imgur.com/IrE36jV.png)

  - Show/hide the *PinListPanel*
    - Press `Tab` (configurable) to toggle the *PinlistPanel* on and off.
    - All pins will initially be listed and pin count shown on the bottom.

  * Filter pins by name
    * Enter text in the input field at the top of the panel.
    * Pins will by filtered by matching text in their name.

  - Center map on pin
    - Left-click on the target pin row and the map will center onto that pin
    - Scrolling animation can be disabled by setting `CenterMap.lerpDuration` config to 0.

  * Reposition the *PinListPanel*
    * Left-click on an open space on the panel and drag to reposition.

  - Resize the *PinListPanel*
    - Hover near the **bottom-right corner** to show the resize button.
    - Left-click and drag on this button to resize the panel.

### PinFilterPanel

  * Filter pins on the map by **any icon type** (replaces vanilla panel).

    ![Pinnacle - PinFilterPanel](https://imgur.com/fPs7fDd.png)

### Add Pin Shortcut

  * You can now use a keyboard-shortcut for adding a Pin!
  * Set the shortcut in ConfigurationManager under `Minimap.Actions` > `addPinAtMouseShortcut`.

### Map Teleporting

  * **Requirements!**
    * `devcommands` must be enabled via console.
    * You must be in single-player mode or the local server-host.

  - Teleport to map point
    - Hold `LeftShift` and click on the target point on the map.
    - Note: this will use the vanilla `GetClosestPin()` logic to check if a pin-position can be used instead.

  * Teleport to pin position
    * Hold `LeftShift` and click on the target row in the *PinListPanel*.

### Configuration

  * Important/critical configuration options are available (more to come later).

    ![Pinnacle - Configuration](https://imgur.com/DBUH4Jq.png)

  - Changing the Minimap.Pin font/font-size
    - These two options are available once you are logged into any world.

## Notes

  * See source at: [GitHub](https://github.com/redseiko/ComfyMods/tree/main/Pinnacle)
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
  * Pinnacle icon created by [@jenniely](https://twitter.com/jenniely) ([jenniely.com](https://jenniely.com))
