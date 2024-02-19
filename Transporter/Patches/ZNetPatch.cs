namespace Transporter;

using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

[HarmonyPatch(typeof(ZNet))]
static class ZNetPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNet.Start))]
  static void StartPostfix() {
    RequestManager.Instance.LoadPendingRequests();
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZNet.OnDestroy))]
  static void OnDestroyPrefix() {
    RequestManager.Instance.SavePendingRequests();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZNet.RPC_PeerInfo))]
  static IEnumerable<CodeInstruction> RPC_PeerInfoTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ZDOMan), nameof(ZDOMan.AddPeer))))
          .Advance(offset: 1)
          .InsertAndAdvance(
              new CodeInstruction(OpCodes.Ldloc_0),
              Transpilers.EmitDelegate(AccessUtils.RegisterRPCs))
          .InstructionEnumeration();
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNet.UpdatePlayerList))]
  static void UpdatePlayerListPostfix(ZNet __instance) {
    PlayerUtils.RefreshPlayerIdMapping();
    TeleportManager.ProcessPendingTeleports();
  }
}
