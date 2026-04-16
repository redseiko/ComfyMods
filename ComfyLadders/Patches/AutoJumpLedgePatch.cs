namespace ComfyLadders;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(AutoJumpLedge))]
static class AutoJumpLedgePatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(AutoJumpLedge.OnTriggerStay))]
  static IEnumerable<CodeInstruction> OnTriggerStayTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Callvirt),  // UnityEngine.Component.GetComponent<Character>
            new CodeMatch(OpCodes.Stloc_0),
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Call))      // UnityEngine.Object.op_Implicit
        .ThrowIfInvalid($"Could not patch AutoJumpLedge.OnTriggerStay()! (character-check)")
        .Advance(offset: 3)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(AutoJumpLedgePatch), nameof(CharacterCheckDelegate))))
        .InstructionEnumeration();
  }

  static Character CharacterCheckDelegate(Character character, AutoJumpLedge autoJumpLedge) {
    if (character
        && character == Player.m_localPlayer
        && IsModEnabled.Value
        && LadderManager.IsEligibleLadder(autoJumpLedge)) {
      LegacyAutoJump.OnAutoJump(autoJumpLedge, character);
      return default;
    }

    return character;
  }
}
