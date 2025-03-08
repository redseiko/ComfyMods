namespace BetterZeeLog;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ZSteamSocket))]
static class ZSteamSocketPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZSteamSocket.SendQueuedPackages))]
  static IEnumerable<CodeInstruction> SendQueuedPackagesTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    if (RemoveFailedToSendDataLogging.Value) {
      return new CodeMatcher(instructions, generator)
          .Start()
          .MatchStartForward(
              new CodeMatch(OpCodes.Ldstr, "Failed to send data "))
          .ThrowIfNotMatch($"Could not patch ZSteamSocket.SendQueuedPackages()! (ldstr-Log)")
          .ExtractLabels(out List<Label> logLabels)
          .MatchStartForward(
              new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZLog), nameof(ZLog.Log))))
          .ThrowIfNotMatch($"Could not patch ZSteamSocket.SendQueuedPackages()! (zLog-Log)")
          .Advance(offset: 1)
          .AddLabels(logLabels)
          .InstructionEnumeration();
    }

    return instructions;
  }
}
