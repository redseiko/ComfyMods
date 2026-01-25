namespace PostalCode;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

[HarmonyPatch(typeof(WaterVolume))]
static class WaterVolumePatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(WaterVolume.GetWaterSurface))]
  static IEnumerable<CodeInstruction> GetWaterSurfaceTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_1),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.LengthXZ))),
            new CodeMatch(OpCodes.Ldc_R4, 10500f))
        .ThrowIfInvalid("Could not patch WaterVolume.GetWaterSurface()! (water-surface-world-edge)")
        .Advance(offset: 2)
        .SetOperandAndAdvance(20000f)
        //.InsertAndAdvance(
        //    new CodeInstruction(
        //        OpCodes.Call, AccessTools.Method(typeof(WaterVolumePatch), nameof(WaterSurfaceWorldEdgeDelegate))))
        .InstructionEnumeration();
  }

  static float WaterSurfaceWorldEdgeDelegate(float length) {
    if (length < 10750f) {
      return length;
    }

    return 0;
  }
}
