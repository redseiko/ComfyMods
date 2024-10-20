namespace ZoneScouter;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Hud))]
static class HudPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.Awake))]
  static void AwakePostfix() {
    if (IsModEnabled.Value) {
      SectorInfoPanelController.ToggleSectorInfoPanel();
      SectorBoundaries.ToggleSectorBoundaries();
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.Update))]
  static void UpdatePostfix() {
    if (IsModEnabled.Value && ToggleSectorBoundariesShortcut.Value.IsDown()) {
      ShowSectorBoundaries.Value = !ShowSectorBoundaries.Value;
    }
  }
}
