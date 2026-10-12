namespace SearsCatalog;

using System;

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
  [HarmonyPatch(nameof(BuildUi.UpdatePieceButtons))]
  static bool UpdatePieceButtonsPrefix(BuildUi __instance) {
    if (IsModEnabled.Value) {
      BuildUiController.UpdatePieceButtons(__instance);
      return false;
    }

    return true;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BuildUi.UpdateSearch))]
  static bool UpdateSearchPrefix(BuildUi __instance, string term) {
    if (IsModEnabled.Value) {
      BuildUiController.UpdateSearch(__instance, term);
      return false;
    }

    return true;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BuildUi.UpdatePieceSelection))]
  static bool UpdatePieceSelectionPrefix(BuildUi __instance, Piece selectedPiece) {
    if (IsModEnabled.Value) {
      BuildUiController.UpdatePieceSelection(__instance, selectedPiece);
      return false;
    }

    return true;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BuildUi.FocusPiece))]
  static bool FocusPiecePrefix(BuildUi __instance, BuildUiPieceButton button) {
    if (IsModEnabled.Value) {
      if (button != null && button.Piece != null) {
        int index = BuildUiController.FilteredPieces.IndexOf(button.Piece);
        if (index >= 0) {
          BuildUiController.PieceGridView?.ScrollToItem(index, extraPadding: 66f);
          return false;
        }
      }

      if (button != null) {
        ScrollRectUtils.EnsureVisibility(
            __instance.m_pieceScrollRect, button.GetComponent<RectTransform>(), padding: 66f);
      }

      return false;
    }

    return true;
  }
}

[HarmonyPatch(typeof(BuildUiPieceButton))]
static class BuildUiPieceButtonPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(BuildUiPieceButton.Setup))]
  static void SetupPrefix(BuildUiPieceButton __instance) {
    Localization.OnLanguageChange =
        (Action) Delegate.Remove(
            Localization.OnLanguageChange, new Action(__instance.RefreshSearchTerm));
  }
}
