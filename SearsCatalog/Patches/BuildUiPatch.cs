namespace SearsCatalog;

using System.Collections.Generic;
using System.Reflection.Emit;

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

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BuildUi.OnFavoritePieceAdded))]
  static bool OnFavoritePieceAddedPrefix(string pieceName) {
    if (IsModEnabled.Value) {
      BuildUiController.RefreshFavoritePiece(pieceName);
      return false;
    }

    return true;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(BuildUi.OnFavoritePieceRemoved))]
  static bool OnFavoritePieceRemovedPrefix(string pieceName) {
    if (IsModEnabled.Value) {
      BuildUiController.RefreshFavoritePiece(pieceName);
      return false;
    }

    return true;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(BuildUi.RemoveFromFavorites))]
  static IEnumerable<CodeInstruction> RemoveFromFavoritesTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(BuildUi), nameof(BuildUi.m_currentBuildTool))),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PieceTable), nameof(PieceTable.m_hideAdvancedMenu))))
        .ThrowIfInvalid($"Could not patch BuildUi.RemoveFromFavorites()! (hide-advanced-menu)")
        .Advance(offset: 3)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(BuildUiPatch), nameof(HideAdvancedMenuDelegate))))
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(BuildUi.ToggleFavorite))]
  static IEnumerable<CodeInstruction> ToggleFavoriteTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(BuildUi), nameof(BuildUi.m_currentBuildTool))),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PieceTable), nameof(PieceTable.m_hideAdvancedMenu))))
        .ThrowIfInvalid($"Could not patch BuildUi.ToggleFavorite()! (hide-advanced-menu)")
        .Advance(offset: 3)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(BuildUiPatch), nameof(HideAdvancedMenuDelegate))))
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(BuildUi.UpdateTagButtons))]
  static IEnumerable<CodeInstruction> UpdateTagButtonsTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Stloc_1),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldloc_1))
        .ThrowIfInvalid("Could not patch BuildUi.UpdateTagButtons()! (show-tags)")
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BuildUiPatch), nameof(ShowTagsDelegate))),
            new CodeInstruction(OpCodes.Stloc_1))
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PieceTable), nameof(PieceTable.m_hideAdvancedMenu))),
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Ceq),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(BuildUi), nameof(BuildUi.SetTagListActive))))
        .ThrowIfInvalid("Could not patch BuildUi.UpdateTagButtons()! (set-tag-list-active)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(BuildUiPatch), nameof(HideAdvancedMenuDelegate))))
        .InstructionEnumeration();
  }

  static bool ShowTagsDelegate(bool showTagsValue) {
    if (!showTagsValue && IsModEnabled.Value && BuildUiAlwaysShowAdvancedMenu.Value) {
      return true;
    }

    return showTagsValue;
  }

  static bool HideAdvancedMenuDelegate(bool hideAdvancedMenuValue) {
    if (hideAdvancedMenuValue && IsModEnabled.Value && BuildUiAlwaysShowAdvancedMenu.Value) {
      return false;
    }

    return hideAdvancedMenuValue;
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
