namespace ColorfulPieces;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

using static ColorfulPieces;
using static PluginConfig;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Player.Update))]
  static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Player), nameof(Player.UpdateHover))))
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_1),
            Transpilers.EmitDelegate(UpdateHoverPostDelegate))
        .InstructionEnumeration();
  }

  static void UpdateHoverPostDelegate(bool takeInput) {
    if (takeInput && IsModEnabled.Value && Player.m_localPlayer.TryGetHovering(out GameObject hovering)) {
      if (ChangePieceColorShortcut.Value.IsDown()) {
        OnChangePieceColorShortcut(hovering);
      }

      if (ClearPieceColorShortcut.Value.IsDown()) {
        OnClearPieceColorShortcut(hovering);
      }

      if (CopyPieceColorShortcut.Value.IsDown()) {
        OnCopyPieceColorShortcut(hovering);
      }
    }
  }

  static bool TryGetHovering(this Player player, out GameObject hovering) {
    hovering = player ? player.m_hovering : default;
    return hovering;
  }

  static bool OnChangePieceColorShortcut(GameObject hovering) {
    if (hovering.TryGetComponentInParent(out WearNTear changeTarget)) {
      ChangePieceColorAction(changeTarget);
      return true;
    }

    return false;
  }

  static bool OnClearPieceColorShortcut(GameObject hovering) {
    if (hovering.TryGetComponent(out WearNTear clearTarget)) {
      ClearPieceColorAction(clearTarget);
      return true;
    }

    return false;
  }

  static bool OnCopyPieceColorShortcut(GameObject hovering) {
    return hovering.TryGetComponentInParent(out WearNTear copyTarget) && CopyPieceColorAction(copyTarget.m_nview);
  }
}
