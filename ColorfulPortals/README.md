# ColorfulPortals

  * You can color the activation glow effect of any portal using RGB and HTML color codes!
  * Those without the mod will still see the vanilla yellow/red glow effect.
  * ***Stone portals*** prefabs are fixed to work (though cannot be built and requires serverside mod to connect).

# Instructions

  1. Unzip `ColorfulPortals.dll` to your `/Valheim/BepInEx/plugins/` folder.
  2. In-game, press F1 to bring up the ConfigurationManager and navigate to the ColorfulPortals section.
     * Change the target color using the RGB sliders or using an HTML color code.
  3. Hover over any portal ***that you are the owner of*** and a prompt to change its color will appear.
     * This prompt can be hidden by disabling the `showChangeColorHoverText` setting.
     * Prompt font-size can be configured with the `colorPromptFontSize` setting.
  4. Hit `LeftShift + E` to change the color.

# Changelog

## 1.4.0

  * Added an option to change the font-size for the text prompt on hover.

## 1.3.0

  * Updated for Hearth & Home.
  * Added `PortalLastColoredBy` ZDO property that is set whenever a player changes the portals color.

## 1.2.0

  * Fixed a memory-leak when caching TeleportWorld/Portals.

## 1.1.0

  * Adding configuration setting to hide the 'change color' prompt over a ward.
  * Now saves the target color's **alpha** value to the ZDO and reads/uses this alpha value if present in the ZDO.

## 1.0.0

  * Initial release.