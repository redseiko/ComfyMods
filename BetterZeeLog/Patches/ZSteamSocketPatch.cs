namespace BetterZeeLog;

using System.Collections.Generic;
using System.Reflection.Emit;

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
          .ThrowIfInvalid($"Could not patch ZSteamSocket.SendQueuedPackages()! (Ldstr-Log)")
          .ExtractLabels(out List<Label> logLabels)
          .MatchStartForward(
              new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZLog), nameof(ZLog.Log))))
          .ThrowIfInvalid($"Could not patch ZSteamSocket.SendQueuedPackages()! (ZLog-Log)")
          .Advance(offset: 1)
          .AddLabels(logLabels)
          .InstructionEnumeration();
    }

    return instructions;
  }

  public static CodeMatcher ExtractLabels(this CodeMatcher matcher, out List<Label> labels) {
    labels = new(matcher.Labels);
    matcher.Labels.Clear();
    return matcher;
  }
}
