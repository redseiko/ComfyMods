namespace SearsCatalog;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Hud))]
static class HudPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.Awake))]
  static void AwakePostfix(Hud __instance) {
    if (IsModEnabled.Value) {
      BuildHudController.SetupPieceSelectionWindow(__instance);
      __instance.StartCoroutine(SetupBuildHudPanelDelayed());
    }
  }

  static readonly WaitForSeconds _waitOneSecond = new(seconds: 1f);

  static IEnumerator SetupBuildHudPanelDelayed() {
    yield return _waitOneSecond;
    BuildHudController.SetupBuildHudPanel();
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.TogglePieceSelection))]
  static void TogglePieceSelectionPostfix(Hud __instance) {
    if (IsModEnabled.Value && __instance.m_pieceSelectionWindow.activeSelf) {
      BuildHudController.CenterOnSelectedIndex();
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Hud.UpdatePieceList))]
  static void UpdatePieceListPrefix(Hud __instance, Piece.PieceCategory category, ref bool __state) {
    if (IsModEnabled.Value) {
      __state = __instance.m_lastPieceCategory == category;
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.UpdatePieceList))]
  static void UpdatePieceListPostfix(Hud __instance, bool __state) {
    if (IsModEnabled.Value && BuildHudController.BuildHudNeedIconLayoutRefresh) {
      BuildHudController.BuildHudNeedIconLayoutRefresh = false;

      foreach (Hud.PieceIconData pieceIconData in __instance.m_pieceIcons) {
        pieceIconData.m_go.GetComponent<RectTransform>()
            .SetAnchorMin(Vector2.up)
            .SetAnchorMax(Vector2.up)
            .SetPivot(Vector2.up);
      }
    }

    if (IsModEnabled.Value && (!__state || BuildHudController.BuildHudNeedIconRecenter)) {
      BuildHudController.BuildHudNeedIconRecenter = false;
      BuildHudController.CenterOnSelectedIndex();
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Hud.UpdatePieceList))]
  static IEnumerable<CodeInstruction> UpdatePieceListTranspiler1(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Player), nameof(Player.GetBuildPieces))),
            new CodeMatch(OpCodes.Stloc_0))
        .ThrowIfInvalid($"Could not patch Hud.UpdatePieceList()! (GetBuildPieces)")
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_0),
            Transpilers.EmitDelegate(GetBuildPiecesDelegate))
        .InstructionEnumeration();
  }

  static void GetBuildPiecesDelegate(List<Piece> buildPieces) {
    if (IsModEnabled.Value) {
      BuildHudController.BuildHudRows =
          (buildPieces.Count / BuildHudController.BuildHudColumns)
              + (buildPieces.Count % BuildHudController.BuildHudColumns == 0 ? 0 : 1);
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Hud.UpdatePieceList))]
  static IEnumerable<CodeInstruction> UpdatePieceListTranspiler2(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(15)),
            new CodeMatch(OpCodes.Stloc_1))
        .ThrowIfInvalid($"Could not patch Hud.UpdatePieceList()! (GridColumns)")
        .Advance(offset: 1)
        .InsertAndAdvance(Transpilers.EmitDelegate(GridColumnsDelegate))
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_6),
            new CodeMatch(OpCodes.Stloc_2))
        .ThrowIfInvalid(($"Could not patch Hud.UpdatePieceList()! (GridRows)"))
        .Advance(offset: 1)
        .InsertAndAdvance(Transpilers.EmitDelegate(GridRowsDelegate))
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Hud.UpdatePieceList))]
  static IEnumerable<CodeInstruction> UpdatePieceListTranspiler3(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_1),
            new CodeMatch(OpCodes.Ldloc_2),
            new CodeMatch(OpCodes.Mul))
        .ThrowIfInvalid($"Could not patch Hud.UpdatePieceList()! (PieceIconsCount)")
        .Advance(offset: 3)
        .InsertAndAdvance(Transpilers.EmitDelegate(PieceIconsCountMultiplyDelegate))
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Hud), nameof(Hud.m_pieceIcons))),
            new CodeMatch(
                OpCodes.Callvirt,
                AccessTools.Method(typeof(List<Hud.PieceIconData>), nameof(List<Hud.PieceIconData>.Clear))))
        .ThrowIfInvalid($"Could not patch Hud.UpdatePieceList()! (PieceIconsClear)")
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            Transpilers.EmitDelegate(PieceIconsClearPostDelegate))
        .InstructionEnumeration();
  }

  static int PieceIconsCountMultiplyDelegate(int value) {
    if (IsModEnabled.Value && BuildHudController.BuildHudNeedRefresh) {
      return -1;
    }

    return value;
  }

  static void PieceIconsClearPostDelegate(Hud hud) {
    if (IsModEnabled.Value) {
      hud.m_pieceListRoot.sizeDelta =
          new(
              hud.m_pieceIconSpacing * BuildHudController.BuildHudColumns,
              hud.m_pieceIconSpacing * BuildHudController.BuildHudRows);

      BuildHudController.BuildHudNeedRefresh = false;
      BuildHudController.BuildHudNeedIconLayoutRefresh = true;
      BuildHudController.BuildHudNeedIconRecenter = true;
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Hud.GetSelectedGrid))]
  static IEnumerable<CodeInstruction> GetSelectedGridTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(15)),
            new CodeMatch(OpCodes.Stloc_0))
        .Advance(offset: 1)
        .InsertAndAdvance(Transpilers.EmitDelegate(GridColumnsDelegate))
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Ldc_I4_6),
            new CodeMatch(OpCodes.Stloc_1))
        .Advance(offset: 1)
        .InsertAndAdvance(Transpilers.EmitDelegate(GridRowsDelegate))
        .InstructionEnumeration();
  }

  static int GridColumnsDelegate(int columns) {
    return IsModEnabled.Value ? BuildHudController.BuildHudColumns : columns;
  }

  static int GridRowsDelegate(int rows) {
    return IsModEnabled.Value ? BuildHudController.BuildHudRows : rows;
  }
}
