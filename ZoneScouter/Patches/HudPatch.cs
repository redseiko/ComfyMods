using HarmonyLib;

using static ZoneScouter.PluginConfig;
using static ZoneScouter.ZoneScouter;

namespace ZoneScouter {
  [HarmonyPatch(typeof(Hud))]
  static class HudPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Hud.Awake))]
    static void AwakePostfix() {
      if (IsModEnabled.Value) {
        ToggleSectorInfoPanel();
        SectorBoundaries.ToggleSectorBoundaries();
      }
    }
  }
}
