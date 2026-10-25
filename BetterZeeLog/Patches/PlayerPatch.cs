namespace BetterZeeLog;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Player.PlacePiece))]
  static IEnumerable<CodeInstruction> PlacePieceTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    if (!RemovePlayerPlacePieceLogging.Value) {
      return instructions;
    }

    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Callvirt),
            new CodeMatch(OpCodes.Ldstr, " placed at height "))
        .ThrowIfNotMatch("Could not patch Player.PlacePiece()! (ldstr-placed-at-height)")
        .CreateLabelOffset(offset: 10, out Label logHeightLabel)
        .InsertAndAdvance(new CodeInstruction(OpCodes.Br, logHeightLabel))
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Callvirt),
            new CodeMatch(OpCodes.Ldstr, " placed at world height "))
        .ThrowIfNotMatch("Could not patch Player.PlacePiece()! (ldstr-placed-at-world-height)")
        .CreateLabelOffset(offset: 8, out Label logWorldHeightLabel)
        .ExtractLabels(out List<Label> ldstrWorldHeightLabels)
        .Insert(new CodeInstruction(OpCodes.Br, logWorldHeightLabel))
        .AddLabels(ldstrWorldHeightLabels)
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Player.TryPlacePiece))]
  static IEnumerable<CodeInstruction> TryPlacePieceTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    if (!RemovePlayerTryPlacePieceLogging.Value) {
      return instructions;
    }

    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "Placed "),
            new CodeMatch(OpCodes.Ldarg_1))
        .ThrowIfNotMatch("Could not patch Player.TryPlacePiece()! (ldstr-placed)")
        .CreateLabelOffset(offset: 6, out Label logPlacedLabel)
        .ExtractLabels(out List<Label> ldstrPlacedLabels)
        .Insert(new CodeInstruction(OpCodes.Br, logPlacedLabel))
        .AddLabels(ldstrPlacedLabels)
        .InstructionEnumeration();
  }
}
