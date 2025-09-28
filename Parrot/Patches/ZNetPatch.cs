namespace Parrot;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ZNet))]
static class ZNetPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZNet.CheckForIncommingServerConnections))]
  static IEnumerable<CodeInstruction> CheckForIncommingServerConnectionsTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    if (!AllowParrotServerConnections.Value) {
      return instructions;
    }

    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldloc_1),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZNet), nameof(ZNet.OnNewConnection))))
        .ThrowIfInvalid("Could not patch ZNet.CheckForIncommingServerConnections()! (on-new-connection)")
        .CreateLabel(out Label onNewConnectionLabel)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ZNetPatch), nameof(OnNewConnectionDelegate))),
            new CodeInstruction(OpCodes.Brfalse, onNewConnectionLabel),
            new CodeInstruction(OpCodes.Ret))
        .InstructionEnumeration();
  }

  static bool OnNewConnectionDelegate(ZNetPeer netPeer) {
    if (AllowParrotServerConnections.Value && ConnectionManager.IsSteamGameServer(netPeer)) {
      ConnectionManager.RegisterParrotClient(netPeer);
      return true;
    }

    return false;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZNet.Disconnect))]
  static IEnumerable<CodeInstruction> DisconnectTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    if (!AllowParrotServerConnections.Value) {
      return instructions;
    }

    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZNet), nameof(ZNet.ClearPlayerData))))
        .ThrowIfInvalid($"Could not patch ZNet.Disconnect()! (clear-player-data)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(
                OpCodes.Call,
                AccessTools.Method(typeof(ConnectionManager), nameof(ConnectionManager.RemoveParrotClient))))
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZNet.SendPlayerList))]
  static IEnumerable<CodeInstruction> SendPlayerListTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ZNet), nameof(ZNet.m_players))),
            new CodeMatch(OpCodes.Callvirt),
            new CodeMatch(
                OpCodes.Callvirt, AccessTools.Method(typeof(ZPackage), nameof(ZPackage.Write), [typeof(int)])))
        .ThrowIfInvalid($"Could not patch ZNet.SendPlayerList()! (write-players-count)")
        .Advance(offset: 3)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(ZNetManager), nameof(ZNetManager.AddServerPlayers))))
        .InstructionEnumeration();
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNet.Start))]
  static void StartPostfix(ZNet __instance) {
    ZNetManager.SetupServerPlayer(__instance);
  }
}
