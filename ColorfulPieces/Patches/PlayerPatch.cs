namespace ColorfulPieces;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Player.Update))]
  static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Player), nameof(Player.UpdateHover))))
        .ThrowIfInvalid($"Could not patch Player.Update()! (UpdateHover)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_1),
            Transpilers.EmitDelegate(UpdateHoverPostDelegate))
        .InstructionEnumeration();
  }

  static void UpdateHoverPostDelegate(bool takeInput) {
    if (takeInput && IsModEnabled.Value && Player.m_localPlayer.TryGetHovering(out GameObject hovering)) {
      if (ChangePieceColorShortcut.Value.IsDown()) {
        ShortcutUtils.OnChangePieceColorShortcut(hovering);
      }

      if (ClearPieceColorShortcut.Value.IsDown()) {
        ShortcutUtils.OnClearPieceColorShortcut(hovering);
      }

      if (CopyPieceColorShortcut.Value.IsDown()) {
        ShortcutUtils.OnCopyPieceColorShortcut(hovering);
      }
    }
  }

  static bool TryGetHovering(this Player player, out GameObject hovering) {
    hovering = player ? player.m_hovering : default;
    return hovering;
  }
}
