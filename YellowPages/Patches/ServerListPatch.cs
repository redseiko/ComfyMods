namespace YellowPages;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ServerList))]
static class ServerListPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ServerList.UpdateServerListGui))]
  static IEnumerable<CodeInstruction> UpdateServerListGuiTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(
                OpCodes.Ldflda, AccessTools.Field(typeof(ServerList), nameof(ServerList.m_serverPlayerLimit))),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(int), nameof(int.ToString))))
        .ThrowIfInvalid($"Could not patch ServerList.UpdateServerListGui()! (server-player-limit)")
        .Advance(offset: 3)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_S, 7),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(ServerListPatch), nameof(ServerPlayerLimitDelegate))))
        .InstructionEnumeration();
  }

  static string ServerPlayerLimitDelegate(string limitString, ServerStatus serverStatus) {
    if (IsModEnabled.Value && TryGetMaxPlayersModifier(serverStatus.m_modifiers, out string maxPlayers)) {
      return maxPlayers;
    }

    return limitString;
  }

  static bool TryGetMaxPlayersModifier(List<string> modifiers, out string maxPlayers) {
    maxPlayers = default;

    if (modifiers.Count <= 0) {
      return false;
    }

    string modifier = modifiers[modifiers.Count - 1];
    
    if (modifier.Length < 4) {
      return false;
    }

    if (modifier[0] != 'm' || modifier[1] != 'p' || modifier[2] != ':') {
      return false;
    }

    maxPlayers = modifier.Substring(3);

    return true;
  }
}
