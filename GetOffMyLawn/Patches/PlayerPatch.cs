namespace GetOffMyLawn;

using HarmonyLib;

using System.Collections.Generic;
using System.Reflection.Emit;

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
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Ldfld, typeof(Piece).GetField(nameof(Piece.m_canBeRemoved))))
        .SetInstructionAndAdvance(Transpilers.EmitDelegate(CanBeRemovedDelegate))
        .InstructionEnumeration();
  }

  public static readonly HashSet<string> RemovablePieceOverrides =
      new() {
        "$tool_cart",
        "$ship_longship",
        "$ship_raft",
        "$ship_karve"
      };

  static bool CanBeRemovedDelegate(Piece piece) {
      return piece.m_canBeRemoved || (IsModEnabled.Value && RemovablePieceOverrides.Contains(piece.m_name));
  }
}
