namespace BetterZeeLog;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Container))]
static class ContainerPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Container.RPC_RequestOpen))]
  static IEnumerable<CodeInstruction> RPC_RequestOpenTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    if (RemoveContainerRequestOpenLogging.Value) {
      return new CodeMatcher(instructions, generator)
          .Start()
          .MatchStartForward(
              new CodeMatch(OpCodes.Ldarg_0),
              new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Container), nameof(Container.m_nview))))
          .ThrowIfInvalid("Could not patch Container.RPC_RequestOpen()! (is-owner)")
          .CreateLabel(out Label isOwnerLabel)
          .Start()
          .InsertAndAdvance(
              new CodeInstruction(OpCodes.Br, isOwnerLabel))
          .MatchStartForward(
              new CodeInstruction(OpCodes.Ldstr, "  but im not the owner"),
              new CodeInstruction(OpCodes.Call, AccessTools.Field(typeof(ZLog), nameof(ZLog.Log))))
          .ThrowIfInvalid("Could not patch Container.RPC_RequestOpen()! (not-owner-log)")
          .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
          .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
          .MatchStartForward(
              new CodeInstruction(OpCodes.Ldstr, "  in use"),
              new CodeInstruction(OpCodes.Call, AccessTools.Field(typeof(ZLog), nameof(ZLog.Log))))
          .ThrowIfInvalid("Could not patch Container.RPC_RequestOpen()! (in-use-log)")
          .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
          .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
          .MatchStartForward(
              new CodeInstruction(OpCodes.Ldstr, "  not yours"),
              new CodeInstruction(OpCodes.Call, AccessTools.Field(typeof(ZLog), nameof(ZLog.Log))))
          .ThrowIfInvalid("Could not patch Container.RPC_RequestOpen()! (not-yours-log)")
          .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
          .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
          .InstructionEnumeration();
    }

    return instructions;
  }
}
