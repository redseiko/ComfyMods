# ZoneScouter

*The sector's instance level... it's over 9000!*

## Features

### SectorInfoPanel

  * Shows current position, current sector and ZDOs in sector.

    ![ZoneScouter - SectorInfoPanel](https://i.imgur.com/pCsiBPb.png)

  - Panel can be repositioned by dragging it with the mouse.

#### CopyPositionButton

  * Open the in-game `Menu` and click on the `Copy` button to copy your current position to clipboard.

### SectorZdoCountGrid

  - Disabled by default, toggle it on/off in ConfigurationManager.
  - There are two GridSize options available (in dropdown menu):  
    * Show ZDO counts for current sector and surrounding sectors (ThreeByThree).  
    
      ![ZoneScouter - SectorZdoCountGrid - 3x3](https://i.imgur.com/gy1cQUh.png)

    * Show ZDO counts for all sectors within range (FiveByFive).  
    
      ![ZoneScouter - SectorZdoCountGrid - 5x5](https://i.imgur.com/2XfS7SC.png)

### ShowSectorBoundaries

* Disabled by default, toggle it on/off in ConfigurationManager.
* Creates semi-transparent colored boundary walls on each side of current sector.  
  \
  ![ZoneScouter - ShowSectorBoundaries](https://i.imgur.com/Ux9Uwqw.png)

### Configuration

  * Changes to all settings will take effect immediately .
  * Use [BepInEx ConfigurationManager](https://thunderstore.io/c/valheim/p/ComfyDeps/BepInExConfigurationManager/) to
    modify settings in-game.

`[_Global]`

  * `isModEnabled`
    * Globally enable or disable this mod.
    * Default: `true`

<details>
<summary><b>SectorInfoPanel</b> Configuration Options (<i>click to expand</i>)</summary>
<br />

`[SectorInfoPanel]`

  * `showSectorInfoPanel`
    * Show the `SectorInfoPanel` on the `Hud`.
    * Default: `true`
  * `sectorInfoPanelPosition`
    * `SectorInfoPanel` position (relative to pivot/anchors).
    * Default: `(0, -25)`
  * `sectorInfoPanelBackgroundColor`
    * `SectorInfoPanel` background color.
    * Default: `#000000E6`
  * `toggleSectorInfoPanelShortcut`
    * Shortcut to toggle on/off the `SectorInfoPanel`.
    * Default: `None`

`[SectorInfoPanel.Font]`

  * `sectorInfoPanelFontSize`
    * `SectorInfoPanel` font size.
    * Default: `16`

`[SectorInfoPanel.PositionRow]`

  * `positionValueXTextColor`
    * `SectorInfoPanel.PositionRow.X` value text color.
    * Default: `#FFE082FF`
  * `positionValueXBackgroundColor`
    * `SectorInfoPanel.PositionRow.X` value background color.
    * Default: `#FFE0821A`
  * `positionValueYTextColor`
    * `SectorInfoPanel.PositionRow.Y` value text color.
    * Default: `#90CAF9FF`
  * `positionValueYBackgroundColor`
    * `SectorInfoPanel.PositionRow.Y` value background color.
    * Default: `#90CAF91A`
  * `positionValueZTextColor`
    * `SectorInfoPanel.PositionRow.Z` value text color.
    * Default: `#A5D6A7FF`
  * `positionValueZBackgroundColor`
    * `SectorInfoPanel.PositionRow.Z` value background color.
    * Default: `#A5D6A71A`

`[SectorInfoPanel.CopyPosition]`

  * `copyPositionValuePrefix`
    * Prefix to prepend to position text when copied to clipboard.
    * Default: `Position: `
  * `copyPositionValueSeparator`
    * Separator to use between the position XYZ values when copied to clipboard.
    * Options: `Space`, `Comma`
    * Default: `Space`
  * `copyPositionValueOrder`
    * Order of the position XYZ values when copied to clipboard.
    * Options: `XYZ`, `XZY`
    * Default: `XYZ`

`[SectorInfoPanel.SectorContent]`

  * `showSectorContent`
    * Show `SectorInfoPanel.Sector` content.
    * Default: `true`

`[SectorInfoPanel.ZDOManagerContent]`

  * `showZDOManagerContent`
    * Show `SectorInfoPanel.ZDOManager` content.
    * Default: `false`

</details>

<details>
<summary><b>SectorZdoCountGrid</b> Configuration Options (<i>click to expand</i>)</summary>
<br />

`[SectorZdoCountGrid]`

  * `showSectorZdoCountGrid`
    * Show the `SectorZdoCount` grid in the `SectorInfo` panel.
    * Default: `false`
  * `sectorZdoCountGridSize`
    * Size of the `SectorZdoCount` grid.
    * Options: `ThreeByThree`, `FiveByFive`
    * Default: `ThreeByThree`
  * `cellZdoCountBackgroundImageColor`
    * `SectorZdoCountCell.ZdoCount.Background.Image` color.
    * Default: `#00000000`
  * `cellZdoCountTextFontSize`
    * `SectorZdoCountCell.ZdoCount.Text` font size.
    * Default: `16`
  * `cellZdoCountTextColor`
    * `SectorZdoCountCell.ZdoCount.Text` color.
    * Default: `#FFFFFFFF`
  * `cellSectorBackgroundImageColor`
    * `SectorZdoCountCell.Sector.Background.Image` color.
    * Default: `#80808080`
  * `cellSectorTextFontSize`
    * `SectorZdoCountCell.Sector.Text` font size.
    * Default: `16`
  * `cellSectorTextColor`
    * `SectorZdoCountCell.Sector.Text` color.
    * Default: `#E6E6E6FF`

</details>

<details>
<summary><b>SectorBoundary</b> Configuration Options (<i>click to expand</i>)</summary>
<br />

`[SectorBoundary]`

  * `showSectorBoundaries`
    * Shows sector boundaries using semi-transparent walls at each boundary.
    * Default: `false`
  * `sectorBoundaryColor`
    * Color to use for the sector boundary walls.
    * Default: `#FF00FFFF`
  * `toggleSectorBoundariesShortcut`
    * Shortcut to toggle on/off sector boundaries.
    * Default: `None`

</details>

## Notes

  * See source at: [GitHub/ComfyMods](https://github.com/redseiko/ComfyMods/tree/main/ZoneScouter).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
