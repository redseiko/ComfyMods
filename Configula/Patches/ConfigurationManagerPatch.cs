using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static Configula.PluginConfig;

namespace Configula {
  [HarmonyPatch(typeof(ConfigurationManager.ConfigurationManager))]
  static class ConfigurationManagerPatch {
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ConfigurationManager.ConfigurationManager.CalculateWindowRect))]
    static IEnumerable<CodeInstruction> CalculateWindowRectTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UnityEngine.Screen), "get_width")),
              new CodeMatch(OpCodes.Ldc_I4))
          .ThrowIfInvalid("Could not patch CalculateWindowRect() width value!")
          .Advance(offset: 2)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(GetWidthDelegate))
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UnityEngine.Screen), "get_height")),
              new CodeMatch(OpCodes.Ldc_I4_S),
              new CodeMatch(OpCodes.Sub))
          .ThrowIfInvalid("Could not patch CalculateWindowRect() height value!")
          .Advance(offset: 2)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(GetHeightDelegate))
          .InstructionEnumeration();
    }

    static int GetWidthDelegate(int width) {
      return IsModEnabled.Value ? WindowWidth.Value : width;
    }

    static int GetHeightDelegate(int heightOffset) {
      return IsModEnabled.Value ? WindowHeightOffset.Value : heightOffset;
    }
  }
}
