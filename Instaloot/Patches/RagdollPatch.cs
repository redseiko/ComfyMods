using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static Instaloot.PluginConfig;

namespace Instaloot {
  [HarmonyPatch(typeof(Ragdoll))]
  static class RagdollPatch {
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Ragdoll.Awake))]
    static IEnumerable<CodeInstruction> AwakeTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Ragdoll), nameof(Ragdoll.m_ttl))))
          .ThrowIfInvalid("Could not patch Ragdoll.Awake() DestroyNow m_ttl value!")
          .Advance(offset: 1)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<float, float>>(DestroyNowDelegate))
          .InstructionEnumeration();
    }

    static float DestroyNowDelegate(float ttl) {
      if (IsModEnabled.Value) {
        return 0f;
      }

      return ttl;
    }
  }
}
