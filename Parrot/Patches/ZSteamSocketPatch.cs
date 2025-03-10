namespace Parrot;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using Steamworks;

[HarmonyPatch(typeof(ZSteamSocket))]
static class ZSteamSocketPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(typeof(ZSteamSocket), MethodType.Constructor, [typeof(SteamNetworkingIPAddr)])]
  static IEnumerable<CodeInstruction> ConstructorTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Ldnull),
            new CodeMatch(OpCodes.Call))
        .ThrowIfInvalid($"Could not patch ZSteamSocket.Constructor()! (connect-by-ip-address")
        .Advance(offset: 2)
        .SetOperandAndAdvance(
            AccessTools.Method(
                typeof(SteamGameServerNetworkingSockets),
                nameof(SteamGameServerNetworkingSockets.ConnectByIPAddress)))
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZSteamSocket.GetConnectionQuality))]
  static IEnumerable<CodeInstruction> GetConnectionQualityTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(OpCodes.Ldloca_S),
            new CodeMatch(OpCodes.Call))
        .ThrowIfInvalid($"Could not patch ZSteamSocket.GetConnectionQuality()! (get-connection-real-time-status)")
        .Advance(offset: 2)
        .SetOperandAndAdvance(
            AccessTools.Method(
                typeof(SteamGameServerNetworkingSockets),
                nameof(SteamGameServerNetworkingSockets.GetConnectionRealTimeStatus)))
        .InstructionEnumeration();
  }
}
