using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static SearsCatalog.PluginConfig;

namespace SearsCatalog {
  [HarmonyPatch(typeof(Player))]
  static class PlayerPatch {
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Player.UpdateBuildGuiInput))]
    static IEnumerable<CodeInstruction> UpdateBuildGuiInputTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZInput), nameof(ZInput.GetMouseScrollWheel))),
              new CodeMatch(OpCodes.Ldc_R4))
          .ThrowIfInvalid("Could not patch Player.UpdateBuildGuiInput()! (PrevCategory)")
          .Advance(offset: 1)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<float, float>>(GetAxisDelegate))
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZInput), nameof(ZInput.GetMouseScrollWheel))),
              new CodeMatch(OpCodes.Ldc_R4))
          .ThrowIfInvalid("Could not patch Player.UpdateBuildGuiInput()! (NextCategory)")
          .Advance(offset: 1)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<float, float>>(GetAxisDelegate))
          .InstructionEnumeration();
    }

    static float GetAxisDelegate(float result) {
      if (result != 0f && IsModEnabled.Value) {
        return 0f;
      }

      return result;
    }
  }
}
