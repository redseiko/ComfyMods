namespace ZoneScouter;

using ComfyLib;

using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

[HarmonyPatch(typeof(Menu))]
static class MenuPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Menu.Start))]
  static void StartPostfix(ref Menu __instance) {
    if (!IsModEnabled.Value) {
      return;
    }

    foreach (GameObject child in __instance.m_menuDialog.gameObject.Children()) {
      if (child.name.StartsWith("darken") && child.TryGetComponent(out Image image)) {
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
