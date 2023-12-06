using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using UnityEngine;

using static SearsCatalog.PluginConfig;

namespace SearsCatalog {
  [HarmonyPatch(typeof(PieceTable))]
  static class PieceTablePatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(PieceTable.SetCategory))]
    static void SetCategoryPostfix() {
      if (IsModEnabled.Value) {
        SearsCatalog.BuildHudNeedRefresh = true;
      }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PieceTable.PrevCategory))]
    static void PrevCategoryPostfix() {
      if (IsModEnabled.Value) {
        SearsCatalog.BuildHudNeedRefresh = true;
      }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PieceTable.NextCategory))]
    static void NextCategoryPostfix() {
      if (IsModEnabled.Value) {
        SearsCatalog.BuildHudNeedRefresh = true;
      }
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(PieceTable.GetPiece), typeof(int), typeof(Vector2Int))]
    static IEnumerable<CodeInstruction> GetPieceTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldarga_S),
              new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Vector2Int), "get_y")),
              new CodeMatch(OpCodes.Ldc_I4_S))
          .ThrowIfInvalid("Could not patch PieceTable.GetPiece()!")
          .Advance(offset: 3)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(GetPieceGetYPostDelegate))
          .InstructionEnumeration();
    }

    static int GetPieceGetYPostDelegate(int value) {
      return IsModEnabled.Value ? SearsCatalog.BuildHudColumns : value;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(PieceTable.GetPieceIndex))]
    static IEnumerable<CodeInstruction> GetPieceIndexTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(15)))
          .ThrowIfInvalid("Could not patch PieceTable.GetPieceIndex()! (Column A)")
          .Advance(offset: 1)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(GetPieceIndexColumnPostDelegate))
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(15)))
          .ThrowIfInvalid("Could not patch PieceTable.GetPieceIndex()! (Column B)")
          .Advance(offset: 1)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(GetPieceIndexColumnPostDelegate))
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(15)))
          .ThrowIfInvalid("Could not patch PieceTable.GetPieceIndex()! (Column C)")
          .Advance(offset: 1)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(GetPieceIndexColumnPostDelegate))
          .InstructionEnumeration();
    }

    static int GetPieceIndexColumnPostDelegate(int value) {
      return IsModEnabled.Value ? SearsCatalog.BuildHudColumns : value;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(PieceTable.LeftPiece))]
    static IEnumerable<CodeInstruction> LeftPieceTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(14)))
          .Advance(offset: 1)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(LeftPieceDelegate))
          .InstructionEnumeration();
    }

    static int LeftPieceDelegate(int value) {
      return IsModEnabled.Value ? SearsCatalog.BuildHudColumns - 1 : value;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(PieceTable.RightPiece))]
    static IEnumerable<CodeInstruction> RightPieceTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(15)))
          .Advance(offset: 1)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(RightPieceDelegate))
          .InstructionEnumeration();
    }

    static int RightPieceDelegate(int value) {
      return IsModEnabled.Value ? SearsCatalog.BuildHudColumns : value;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(PieceTable.UpPiece))]
    static IEnumerable<CodeInstruction> UpPieceTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldc_I4_5))
          .Advance(offset: 1)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(UpPieceDelegate))
          .InstructionEnumeration();
    }

    static int UpPieceDelegate(int value) {
      return IsModEnabled.Value ? SearsCatalog.BuildHudRows - 1 : value;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(PieceTable.DownPiece))]
    static IEnumerable<CodeInstruction> DownPieceTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldc_I4_6))
          .Advance(offset: 1)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(DownPieceDelegate))
          .InstructionEnumeration();
    }

    static int DownPieceDelegate(int value) {
      return IsModEnabled.Value ? SearsCatalog.BuildHudRows : value;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PieceTable.LeftPiece))]
    [HarmonyPatch(nameof(PieceTable.RightPiece))]
    [HarmonyPatch(nameof(PieceTable.UpPiece))]
    [HarmonyPatch(nameof(PieceTable.DownPiece))]
    static void ControllerPiecePostfix() {
      if (IsModEnabled.Value) {
        SearsCatalog.CenterOnSelectedIndex();
      }
    }
  }
}
