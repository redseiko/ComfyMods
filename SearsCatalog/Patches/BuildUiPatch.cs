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
  static bool FocusPiecePrefix(BuildUiPieceButton button) {
    if (IsModEnabled.Value) {
      BuildUiController.FocusPiece(button.Piece);
      return false;
    }

    return true;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BuildUi.ConfigureButtonNavigation))]
  static bool ConfigureButtonNavigationPrefix(BuildUi __instance) {
    if (IsModEnabled.Value) {
      BuildUiController.ConfigureButtonNavigation(__instance);
      return false;
    }

    return true;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BuildUi.GetFirstPieceButton))]
  static bool GetFirstPieceButtonPrefix(ref GameObject __result) {
    if (IsModEnabled.Value) {
      __result = BuildUiController.GetFirstPieceButton();
      return false;
    }

    return true;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BuildUi.SelectFirstPiece))]
  static bool SelectFirstPiecePrefix() {
    if (IsModEnabled.Value) {
      BuildUiController.SelectFirstPiece();
      return false;
    }

    return true;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BuildUi.SelectFirstAvailableButton))]
  static bool SelectFirstAvailableButtonPrefix(BuildUi __instance) {
    if (IsModEnabled.Value) {
      BuildUiController.SelectFirstAvailableButton(__instance);
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
    Localization.OnLanguageChange -= __instance.RefreshSearchTerm;
  }
}
