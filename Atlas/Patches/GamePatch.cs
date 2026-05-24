namespace Atlas;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Game))]
static class GamePatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Game.FixedUpdate))]
  static IEnumerable<CodeInstruction> FixedUpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    if (!OverrideServerPosition.Value) {
      return instructions;
    }

    Vector3 position = CustomServerPosition.Value;

    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_R4, 1000000f),
            new CodeMatch(OpCodes.Ldc_R4, 0f),
            new CodeMatch(OpCodes.Ldc_R4, 1000000f))
        .ThrowIfInvalid("Could not patch Game.FixedUpdate()! (set-reference-position)")
        .SetInstruction(new CodeInstruction(OpCodes.Ldc_R4, position.x))
        .Advance(offset: 2)
        .SetInstruction(new CodeInstruction(OpCodes.Ldc_R4, position.y))
        .InstructionEnumeration();
  }
}
