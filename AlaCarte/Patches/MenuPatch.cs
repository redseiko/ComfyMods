namespace AlaCarte;

using System;

using ComfyLib;

using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

[HarmonyPatch(typeof(Menu))]
static class MenuPatch {
  static RectTransform _menuDialogVanilla;
  static RectTransform _menuDialogOld;

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Menu.Start))]
  static void StartPostfix(ref Menu __instance) {
    _menuDialogVanilla = __instance.m_menuDialog.GetComponent<RectTransform>();
    _menuDialogOld = __instance.m_root.Find("OLD_menu").GetComponent<RectTransform>();

    SetupMenuDialogVanilla(_menuDialogVanilla);
    SetupMenuDialogOld(_menuDialogOld);
  }

  static void SetupMenuDialogVanilla(RectTransform menuTransform) {
    Image image = menuTransform.Find("darken").Ref()?.GetComponent<Image>();
    image.raycastTarget = false;

    PanelDragger dragger = menuTransform.Find("ornament").Ref()?.gameObject.AddComponent<PanelDragger>();
    dragger.TargetRectTransform = menuTransform;
    dragger.OnPanelEndDrag += OnMenuDialogEndDrag;
  }

  static void SetupMenuDialogOld(RectTransform menuTransform) {
    PanelDragger dragger = menuTransform.gameObject.AddComponent<PanelDragger>();
    dragger.TargetRectTransform = menuTransform;
    dragger.OnPanelEndDrag += OnMenuDialogEndDrag;
  }

  static void OnMenuDialogEndDrag(object sender, Vector3 position) {
    MenuDialogPosition.Value = position;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Menu.Show))]
  static void ShowPrefix(ref Menu __instance) {
    RectTransform menuDialogByType =
        MenuDialogType.Value switch {
          DialogType.Vanilla => _menuDialogVanilla,
          DialogType.Old => _menuDialogOld,
          _ => throw new NotImplementedException(),
        };

    if (__instance.m_menuDialog != menuDialogByType) {
      __instance.m_menuDialog.gameObject.SetActive(false);
      __instance.m_menuDialog = menuDialogByType;
    }

    menuDialogByType.anchoredPosition = MenuDialogPosition.Value;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Menu.UpdateNavigation))]
  static bool UpdateNavigationPrefix(ref Menu __instance) {
    if (__instance.m_menuDialog == _menuDialogOld) {
      return false;
    }

    return true;
  }
}
