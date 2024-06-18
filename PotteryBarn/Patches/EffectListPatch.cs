namespace PotteryBarn;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(EffectList))]
static class EffectListPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(EffectList.Create))]
  static IEnumerable<CodeInstruction> CreateTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_2),
            new CodeMatch(
                OpCodes.Ldfld,
                AccessTools.Field(typeof(EffectList.EffectData), nameof(EffectList.EffectData.m_enabled))))
        .ThrowIfInvalid($"Could not patch EffectList.Create()! (m_enabled)")
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_2),
            Transpilers.EmitDelegate(CheckEffectDataDelegate))
        .InstructionEnumeration();
  }

  static void CheckEffectDataDelegate(EffectList.EffectData effectData) {
    if (IsModEnabled.Value && effectData.m_enabled && !effectData.m_prefab) {
      effectData.m_enabled = false;
    }
  }
}
