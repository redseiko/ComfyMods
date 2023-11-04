using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static BetterZeeLog.PluginConfig;

namespace BetterZeeLog {
  [HarmonyPatch(typeof(ZSteamSocket))]
  static class ZSteamSocketPatch {
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ZSteamSocket.SendQueuedPackages))]
    static IEnumerable<CodeInstruction> SendQueuedPackagesTranspiler(IEnumerable<CodeInstruction> instructions) {
      if (RemoveFailedToSendDataLogging.Value) {
        return new CodeMatcher(instructions)
            .MatchForward(
                useEnd: false,
                new CodeMatch(OpCodes.Ldstr, "Failed to send data "),
                new CodeMatch(OpCodes.Ldloca_S),
                new CodeMatch(OpCodes.Constrained),
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(object), nameof(ToString))),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZLog), nameof(ZLog.Log))))
            .Advance(offset: 1)
            .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Pop))
            .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
            .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
            .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
            .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
            .InstructionEnumeration();
      }

      return instructions;
    }
  }
}
