namespace GetOffMyLawn;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(Player.Repair))]
  static bool RepairPrefix(Player __instance, ItemDrop.ItemData toolItem) {
    if (IsModEnabled.Value) {
      __instance.RepairPiece(toolItem, __instance.m_hoveringPiece);
      return false;
    }

    return true;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Player.RemovePiece))]
  static IEnumerable<CodeInstruction> RemovePieceTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldfld, typeof(Piece).GetField(nameof(Piece.m_canBeRemoved))))
        .ThrowIfInvalid($"Could not patch Player.RemovePiece()! (m_canBeRemoved)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_1),
            Transpilers.EmitDelegate(CanBeRemovedDelegate))
        .InstructionEnumeration();
  }

  static bool CanBeRemovedDelegate(bool canBeRemoved, Piece piece) {
    return canBeRemoved || (IsModEnabled.Value && PieceUtils.CanRemovePiece(piece));
  }
}
