namespace Effectual;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Character))]
static class CharacterPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Character.OnDestroy))]
  static IEnumerable<CodeInstruction> OnDestroyTranspiler(IEnumerable<CodeInstruction> instructions) {
    if (!IsModEnabled.Value) {
      return instructions;
    }

    return new CodeMatcher(instructions)
        .Start()
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(
                OpCodes.Call,
                AccessTools.Method(typeof(CharacterPatch), nameof(RemoveFromEffectAreaDelegate))))
        .InstructionEnumeration();
  }

  static void RemoveFromEffectAreaDelegate(Character character) {
    foreach (EffectArea effectArea in EffectArea.s_allAreas) {
      effectArea.m_collidedWithCharacter.Remove(character);
    }
  }
}
