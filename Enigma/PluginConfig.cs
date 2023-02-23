using BepInEx.Configuration;

using UnityEngine;

namespace Enigma {
  public static class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }
    public static ConfigEntry<bool> IsBossAnnouncementEnabled { get; private set; }
    public static void BindConfig(ConfigFile config) {
      IsModEnabled =
          config.Bind(
              "_Global", "isModEnabled", true, "Globally enable or disable this mod (restart required).");

      IsBossAnnouncementEnabled =
         config.Bind(
             "_Global", "isBossAnnouncementEnabled", true, "Enables boss announcement on first sight of that boss (tied to ZDO).");
    }
  }
}
