namespace YellowPages;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using Steamworks;

using static PluginConfig;

[HarmonyPatch(typeof(ZSteamMatchmaking))]
static class ZSteamMatchmakingPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZSteamMatchmaking.OnServerResponded))]
  static IEnumerable<CodeInstruction> OnServerRespondedTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(
                OpCodes.Callvirt, AccessTools.Method(typeof(ServerStatus), nameof(ServerStatus.UpdateStatus))))
        .ThrowIfInvalid($"Could not patch ZSteamMatchmaking.OnServerResponded()! (update-status)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(
                OpCodes.Ldfld, AccessTools.Field(typeof(gameserveritem_t), nameof(gameserveritem_t.m_nMaxPlayers))),
            new CodeInstruction(OpCodes.Ldloc_3),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(ZSteamMatchmakingPatch), nameof(UpdateStatusPostDelegate))))
        .InstructionEnumeration();
  }

  static void UpdateStatusPostDelegate(int maxPlayers, ServerStatus serverStatus) {
    // Steam defaults: 32 (client), 64 (dedicated-server)
    // PlayFab defaults: 10 (client), 11 (dedicated-server)
    if (IsModEnabled.Value && maxPlayers != 32 && maxPlayers != 64) {
      serverStatus.m_modifiers.Add($"mp:{maxPlayers}");
    }
  }
}
