namespace Pseudonym;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(PlayerCustomizaton))]
static class PlayerCustomizatonPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(PlayerCustomizaton.OnBeardLeft))]
  static IEnumerable<CodeInstruction> OnBeardLeftTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Player), nameof(Player.GetPlayerModel))),
            new CodeMatch(OpCodes.Ldc_I4_1))
        .ThrowIfInvalid("Could not patch PlayerCustomizaton.OnBeardLeft()!")
        .Advance(offset: 1)
        .InsertAndAdvance(Transpilers.EmitDelegate(GetPlayerModelDelegate))
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(PlayerCustomizaton.OnBeardRight))]
  static IEnumerable<CodeInstruction> OnBeardRightTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Player), nameof(Player.GetPlayerModel))),
            new CodeMatch(OpCodes.Ldc_I4_1))
        .ThrowIfInvalid("Could not patch PlayerCustomizaton.OnBeardRight()!")
        .Advance(offset: 1)
        .InsertAndAdvance(Transpilers.EmitDelegate(GetPlayerModelDelegate))
        .InstructionEnumeration();
  }

  static int GetPlayerModelDelegate(int value) {
    return IsModEnabled.Value ? 0 : value;
  }
}
