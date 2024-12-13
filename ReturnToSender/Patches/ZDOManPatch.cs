namespace ReturnToSender;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

[HarmonyPatch(typeof(ZDOMan))]
static class ZDOManPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZDOMan.Update))]
  static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZDOMan), nameof(ZDOMan.SendZDOToPeers2))))
        .ThrowIfInvalid($"Could not patch ZDOMan.Update()! (send-zdo-to-peers)")
        .SetOperandAndAdvance(AccessTools.Method(typeof(ZDOManPatch), nameof(SendZDOToPeers)))
        .InstructionEnumeration();
  }

  static void SendZDOToPeers(ZDOMan zdoManager, float dt) {
    zdoManager.m_sendTimer += dt;

    if (zdoManager.m_sendTimer > 0.05f) {
      zdoManager.m_sendTimer = 0f;
      List<ZDOMan.ZDOPeer> peers = zdoManager.m_peers;

      for (int i = 0, count = peers.Count; i < count; i++) {
        zdoManager.SendZDOs(peers[i], flush: false);
      }
    }
  }
}
