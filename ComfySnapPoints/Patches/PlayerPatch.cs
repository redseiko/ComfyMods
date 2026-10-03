namespace ComfySnapPoints;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.PieceRayTest))]
  static void PieceRayTestPostfix(Player __instance, ref Piece piece, bool __result) {
    SnapPointManager.SetLastRayPiece(
        __result && IsModEnabled.Value
            ? piece
            : default);
  }

  [HarmonyEmitIL]
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Player.UpdatePlacementGhost))]
  static IEnumerable<CodeInstruction> UpdatePlacementGhostTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Bge_Un),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Player), nameof(Player.m_manualSnapPoint))),
            new CodeMatch(OpCodes.Ldc_I4_1),
            new CodeMatch(OpCodes.Sub),
            new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(Player), nameof(Player.m_manualSnapPoint))))
        .ThrowIfInvalid($"Could not patch Player.UpdatePlacementGhost()! (manual-snap-point-minus)")
        .SaveOperand(out object branchTabRighLabel)
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(ManualSnapPointMinusDelegate))),
            new CodeInstruction(OpCodes.Brfalse, branchTabRighLabel))
        .InstructionEnumeration();
  }

  static bool ManualSnapPointMinusDelegate(Player player) {
    // TODO
    return false;
  }
}
