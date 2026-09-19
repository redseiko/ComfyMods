namespace SearsCatalog;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(BuildUi))]
static class BuildUiPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(BuildUi.Start))]
  static void StartPostfix(BuildUi __instance) {
    BuildUiController.SetupBuildUi(__instance);
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BuildUi.OnDestroy))]
  static void OnDestroyPrefix(BuildUi __instance) {
    BuildUiController.DestroyBuildUi(__instance);
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BuildUi.OpenBuildMenu))]
  static void OpenBuildMenuPrefix(BuildUi __instance) {
    BuildUiController.ShowBuildUi(__instance);
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BuildUi.FocusPiece))]
  static bool FocusPiecePrefix(BuildUi __instance, BuildUiPieceButton button) {
    if (IsModEnabled.Value) {
      ScrollRectUtils.EnsureVisibility(
          __instance.m_pieceScrollRect, button.GetComponent<RectTransform>(), padding: 66f);

      return false;
    }

    return true;
  }
}
