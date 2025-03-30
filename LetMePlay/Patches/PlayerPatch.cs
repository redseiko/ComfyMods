namespace LetMePlay;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.UpdatePlacementGhost))]
  static void UpdatePlacementGhostPostfix(Player __instance) {
    if (__instance) {
      SetupPlacementMarker(__instance.m_placementMarkerInstance);
    }
  }

  public static void SetupPlacementMarker(GameObject placementMarker) {
    if (placementMarker && IsModEnabled.Value && DisableBuildPlacementMarker.Value) {
      placementMarker.SetActive(false);
    }
  }
}
