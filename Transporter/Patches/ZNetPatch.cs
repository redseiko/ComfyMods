using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

namespace Transporter {
  [HarmonyPatch(typeof(ZNet))]
  static class ZNetPatch {
    static IEnumerable<CodeInstruction> RPC_PeerInfoTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
            .MatchForward(
                useEnd: false,
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ZDOMan), nameof(ZDOMan.AddPeer))))
            .Advance(offset: 1)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_0),
                Transpilers.EmitDelegate<Action<ZNetPeer>>(AccessUtils.RegisterRPCs))
            .InstructionEnumeration();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZNet.UpdatePlayerList))]
    static void UpdatePlayerListPostfix(ZNet __instance) {
      PlayerUtils.RefreshPlayerIdMapping();
      TeleportManager.ProcessPendingTeleports();
    }
  }
}
