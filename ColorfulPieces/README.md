# ColorfulPieces

  * You can color any building piece that can be built using the Hammer using RGB or HTML color codes.
    * Coloring is very simple at the moment and will color all materials/textures on the object (to be refined later).
    * Those without the mod installed will still see the default vanilla materials/textures.

# Instructions

  1. Unzip `ColorfulPieces.dll` to your `/Valheim/BepInEx/plugins/` folder.
  2. In-game, press F1 to bring up the ConfigurationManager and navigate to the ColorfulPieces section.
     * Change the target color using the RGB sliders or using an HTML color code.
     * Change the target emission color factor using the slider (this affects how bright the target color will be).
  3. Hover over any building piece ***that you are the owner of*** and a prompt will appear.
     * Hit `LeftShift + R` to change the building piece color to the target color and emission factor.
     * Hit `LeftAlt + R` to clear any existing colors from the building piece.
     * This prompt can be hidden by disabling the `showChangeRemoveColorPrompt` setting.

# Changelog

## 1.1.0

  * Fixed a memory leak causing the game to crash/freeze during a player profile save.
    * This is because we used ConditionalWeakTable to cache piece materials but the Unity docs state that
      UnityEngine.Object does not support WeakReferences.
    * Changed to a Dictionary instead and patch `WearNTear.OnDestroy()` to remove the reference.
  * Added configuration setting to hide the 'change color' and 'remove color' prompt over a ward.

## 1.0.0

  * Initial release.