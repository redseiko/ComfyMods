using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using static GetOffMyLawn.GetOffMyLawn;
using static GetOffMyLawn.PluginConfig;

using UnityEngine;

namespace GetOffMyLawn {
  [HarmonyPatch(typeof(Player))]
  public class PlayerPatch {
    static readonly HashSet<string> RemovablePieceOverrides =
        new() {
          "$tool_cart",
          "$ship_longship",
          "$ship_raft",
          "$ship_karve"
        };

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Player.Repair))]
    static void RepairPrefix(ref Player __instance, ItemDrop.ItemData toolItem, Piece repairPiece) {
      if (!IsModEnabled.Value || !__instance || !__instance.InPlaceMode()) {
        return;
      }

      Piece hoveringPiece = __instance.m_hoveringPiece;

      if (!hoveringPiece || !__instance.CheckCanRemovePiece(hoveringPiece) || hoveringPiece.GetComponent<Plant>()) {
        return;
      }

      string pieceName = Localization.instance.Localize(hoveringPiece.m_name);

      if (!PrivateArea.CheckAccess(hoveringPiece.transform.position, flash: true)) {
        PluginLogger.LogInfo($"Unable to repair piece '{pieceName}' due to ward in range.");
        return;
      }

      if (!hoveringPiece.m_nview || !hoveringPiece.m_nview.IsValid()) {
        PluginLogger.LogWarning($"Unable to repair piece '{pieceName}' due to invalid ZNetView.");
        return;
      }

      hoveringPiece.m_nview.GetZDO().Set(HealthHashCode, PieceHealth.Value);
      PluginLogger.LogInfo($"Repaired piece '{pieceName}' to health: {PieceHealth.Value}");

      if (ShowTopLeftMessageOnPieceRepair.Value) {
        __instance.Message(
            MessageHud.MessageType.TopLeft, $"Repaired piece '{pieceName}' to health: {PieceHealth.Value}");
      }

      hoveringPiece.GetComponent<WearNTear>().m_lastRepair = Time.time;

    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Player.RemovePiece))]
    static IEnumerable<CodeInstruction> RemovePieceTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldfld, typeof(Piece).GetField(nameof(Piece.m_canBeRemoved))))
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<Piece, bool>>(CanBeRemovedDelegate))
          .InstructionEnumeration();
    }

    static bool CanBeRemovedDelegate(Piece piece) {
      return piece.m_canBeRemoved || (IsModEnabled.Value && RemovablePieceOverrides.Contains(piece.m_name));
    }
  }
}
