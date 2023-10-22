# ZoneScouter

*The sector's instance level... it's over 9000!*

## Features

### SectorInfoPanel

  * Shows current position, current sector and ZDOs in sector.

    ![ZoneScouter - SectorInfoPanel](https://i.imgur.com/pCsiBPb.png)

  - Panel can be repositioned by dragging it with the mouse.

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

  * Almost everything is configurable (more will be added later).
    
    ![ZoneScouter - Configuration](https://i.imgur.com/5ScpxAV.png)

## Notes

  * This is the *good enough* release with more features/options to be added later.
  * See source at: [GitHub](https://github.com/redseiko/ComfyMods/tree/main/ZoneScouter).
  * Looking for a chill Valheim server? [Comfy Valheim Discord](https://discord.gg/ameHJz5PFk)
  * Check out our community driven listing site at: [valheimlist.org](https://valheimlist.org/)

## Changelog

### 1.3.0

  * Updated all `UI.Text` references to use `TextMeshPro`.
  * Added new config option `showZDOManagerContent` to toggle `NextId` row in the `SectorInfoPanel`, default to off.

### 1.2.0

  * Fixed for `v0.216.9` patch.

### 1.1.0

  * Added a new row `ZdoManager.NextId` to the `SectorInfoPanel`.
  * Minor code clean-up.

### 1.0.1

  * Fixed grid display of sectors corresponding to NESW movement. Blah math.

### 1.0.0

  * Initial release.