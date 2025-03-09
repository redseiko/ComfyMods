namespace Transporter;

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
          .Start()
          .MatchStartForward(
              new CodeMatch(OpCodes.Ldloc_1),
              new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ZDOMan), nameof(ZDOMan.AddPeer))))
          .ThrowIfNotMatch($"Could not patch ZNet.RPC_PeerInfo()! (zdoman-add-peer)")
          .Advance(offset: 2)
          .InsertAndAdvance(
              new CodeInstruction(OpCodes.Ldloc_1),
              new CodeInstruction(
                  OpCodes.Call, AccessTools.Method(typeof(AccessUtils), nameof(AccessUtils.RegisterRPCs))))
          .InstructionEnumeration();
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZNet.UpdatePlayerList))]
  static void UpdatePlayerListPostfix(ZNet __instance) {
    PlayerUtils.RefreshPlayerIdMapping();
    TeleportManager.ProcessPendingTeleports();
  }
}
