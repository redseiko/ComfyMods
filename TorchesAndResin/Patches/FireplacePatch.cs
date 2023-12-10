using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static TorchesAndResin.PluginConfig;
using static TorchesAndResin.TorchesAndResin;

namespace TorchesAndResin {
  [HarmonyPatch(typeof(Fireplace))]
  static class FireplacePatch {
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Fireplace.Awake))]
    static IEnumerable<CodeInstruction> AwakeTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldstr, "UpdateFireplace"),
              new CodeMatch(OpCodes.Ldc_R4, 0f),
              new CodeMatch(OpCodes.Ldc_R4, 2f))
          .Advance(offset: 1)
          .SetOperandAndAdvance(0.5f)
          .InstructionEnumeration();
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Fireplace.Awake))]
    static void AwakePrefix(ref Fireplace __instance, ref bool __state) {
      if (IsModEnabled.Value) {
        __state = Array.IndexOf(EligibleTorchItemNames, Utils.GetPrefabName(__instance.gameObject.name)) >= 0;

        if (__state) {
          __instance.m_startFuel = TorchStartingFuel.Value;
        }
      }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Fireplace.Awake))]
    static void AwakePostfix(ref Fireplace __instance, bool __state) {
      if (IsModEnabled.Value && __state && __instance.m_nview && __instance.m_nview.IsOwner()) {
        __instance.m_startFuel = TorchStartingFuel.Value;
        __instance.m_nview.m_zdo.Set(ZDOVars.s_fuel, (float) TorchStartingFuel.Value);
      }
    }
  }
}
