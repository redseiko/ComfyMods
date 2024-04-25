namespace AlaCarte;

using HarmonyLib;

[HarmonyPatch(typeof(Menu))]
static class MenuPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Menu.Start))]
  static void StartPostfix(Menu __instance) {
    MenuUtils.SetupMenuDialogs(__instance);
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Menu.Show))]
  static void ShowPrefix(Menu __instance) {
    MenuUtils.ShowMenuDialog(__instance);
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Menu.UpdateNavigation))]
  static bool UpdateNavigationPrefix(Menu __instance) {
    if (__instance.m_menuDialog == MenuUtils.MenuDialogOld) {
      return false;
    }

    return true;
  }
}
