namespace SearsCatalog;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(PieceTable))]
static class PieceTablePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(PieceTable.SetCategory))]
  static void SetCategoryPostfix() {
    if (IsModEnabled.Value) {
      BuildHudController.BuildHudNeedRefresh = true;
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(PieceTable.PrevCategory))]
  static void PrevCategoryPostfix() {
    if (IsModEnabled.Value) {
      BuildHudController.BuildHudNeedRefresh = true;
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(PieceTable.NextCategory))]
  static void NextCategoryPostfix() {
    if (IsModEnabled.Value) {
      BuildHudController.BuildHudNeedRefresh = true;
    }
  }

  [HarmonyPatch]
  static class GetPiecePatch {
    [HarmonyTargetMethod]
    static MethodBase CalculateMethod() {
      return ReflectionUtils.GetFirstMatchingMethod(
          typeof(PieceTable),
          nameof(PieceTable.GetPiece),
          [typeof(int), typeof(Vector2Int)],
          [typeof(Piece.PieceCategory), typeof(Vector2Int)]);
    }

    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> GetPieceTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .Start()
          .MatchStartForward(
              new CodeMatch(OpCodes.Ldarga_S),
              new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Vector2Int), nameof(Vector2Int.y))),
              new CodeMatch(OpCodes.Ldc_I4_S))
          .ThrowIfInvalid("Could not patch PieceTable.GetPiece()! (get-y)")
          .Advance(offset: 3)
          .InsertAndAdvance(
              new CodeInstruction(
                  OpCodes.Call, AccessTools.Method(typeof(GetPiecePatch), nameof(GetPieceGetYDelegate))))
          .InstructionEnumeration();
    }

    static int GetPieceGetYDelegate(int value) {
      return IsModEnabled.Value ? BuildHudController.BuildHudColumns : value;
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(PieceTable.GetPieceIndex))]
  static IEnumerable<CodeInstruction> GetPieceIndexTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(15)))
        .ThrowIfInvalid("Could not patch PieceTable.GetPieceIndex()! (column-a)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(PieceTablePatch), nameof(GetPieceIndexColumnDelegate))))
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(15)))
        .ThrowIfInvalid("Could not patch PieceTable.GetPieceIndex()! (column-b)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(PieceTablePatch), nameof(GetPieceIndexColumnDelegate))))
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(15)))
        .ThrowIfInvalid("Could not patch PieceTable.GetPieceIndex()! (column-c)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(PieceTablePatch), nameof(GetPieceIndexColumnDelegate))))
        .InstructionEnumeration();
  }

  static int GetPieceIndexColumnDelegate(int value) {
    return IsModEnabled.Value ? BuildHudController.BuildHudColumns : value;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(PieceTable.LeftPiece))]
  static IEnumerable<CodeInstruction> LeftPieceTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(14)))
        .ThrowIfInvalid($"Could not patch PieceTable.LeftPiece()! (column)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PieceTablePatch), nameof(LeftPieceDelegate))))
        .InstructionEnumeration();
  }

  static int LeftPieceDelegate(int value) {
    return IsModEnabled.Value ? BuildHudController.BuildHudColumns - 1 : value;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(PieceTable.RightPiece))]
  static IEnumerable<CodeInstruction> RightPieceTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(15)))
        .ThrowIfInvalid("Could not patch PieceTable.RightPiece()! (column)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PieceTablePatch), nameof(RightPieceDelegate))))
        .InstructionEnumeration();
  }

  static int RightPieceDelegate(int value) {
    return IsModEnabled.Value ? BuildHudController.BuildHudColumns : value;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(PieceTable.UpPiece))]
  static IEnumerable<CodeInstruction> UpPieceTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_5))
        .ThrowIfInvalid("Could not patch PieceTable.UpPiece()! (row)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PieceTablePatch), nameof(UpPieceDelegate))))
        .InstructionEnumeration();
  }

  static int UpPieceDelegate(int value) {
    return IsModEnabled.Value ? BuildHudController.BuildHudRows - 1 : value;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(PieceTable.DownPiece))]
  static IEnumerable<CodeInstruction> DownPieceTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_6))
        .ThrowIfInvalid($"Could not patch PieceTable.DownPiece()! (row)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PieceTablePatch), nameof(DownPieceDelegate))))
        .InstructionEnumeration();
  }

  static int DownPieceDelegate(int value) {
    return IsModEnabled.Value ? BuildHudController.BuildHudRows : value;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(PieceTable.LeftPiece))]
  [HarmonyPatch(nameof(PieceTable.RightPiece))]
  [HarmonyPatch(nameof(PieceTable.UpPiece))]
  [HarmonyPatch(nameof(PieceTable.DownPiece))]
  static void ControllerPiecePostfix() {
    if (IsModEnabled.Value) {
      BuildHudController.CenterOnSelectedIndex();
    }
  }
}
