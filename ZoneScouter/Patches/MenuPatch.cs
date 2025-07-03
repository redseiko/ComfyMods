namespace ZoneScouter;

using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

[HarmonyPatch(typeof(Menu))]
static class MenuPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Menu.Start))]
  static void StartPostfix(Menu __instance) {
    if (IsModEnabled.Value) {
      SetupMenu(__instance);
    }
  }

  public static void SetupMenu(Menu menu) {
    foreach (Transform child in menu.m_menuDialog) {
      if (child.name.StartsWith("darken", System.StringComparison.InvariantCulture)
          && child.TryGetComponent(out Image image)) {
        image.raycastTarget = false;
      }
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Menu.Show))]
  static void ShowPostfix() {
    if (IsModEnabled.Value) {
      SectorInfoPanelController.SectorInfoPanel?.ToggleCopyButtons(true);
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Menu.Hide))]
  static void HidePostfix() {
    if (IsModEnabled.Value) {
      SectorInfoPanelController.SectorInfoPanel?.ToggleCopyButtons(false);
    }
  }
}
