using BepInEx;

using HarmonyLib;

using Steamworks;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace NagleNoMore {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class NagleNoMore : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.naglenomore";
    public const string PluginName = "NagleNoMore";
    public const string PluginVersion = "1.3.1";

    Harmony _harmony;

    public void Awake() {
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    [HarmonyPatch(typeof(ZSteamSocket))]
    class ZSteamSocketPatch {
      [HarmonyTranspiler]
      [HarmonyPatch(nameof(ZSteamSocket.SendQueuedPackages))]
      static IEnumerable<CodeInstruction> SendQueuedPackagesTranspiler(IEnumerable<CodeInstruction> instructions) {
        // nSendFlags:
        //   * k_nSteamNetworkingSend_NoNagle = 1
        //   * k_nSteamNetworkingSend_Reliable = 8
        return new CodeMatcher(instructions)
            .MatchForward(
                useEnd: false,
                new CodeMatch(OpCodes.Ldc_I4_8), // k_nSteamNetworkingSend_Reliable
                new CodeMatch(OpCodes.Ldloca_S), // out num
                new CodeMatch(OpCodes.Call),     // ... SteamNetworkingSockets.SendMessageToConnection(...)
                new CodeMatch(OpCodes.Stloc_3))  // EResult result = ...
            .SetAndAdvance(OpCodes.Ldc_I4, 9)    // k_nSteamNetworkingSend_NoNagle | k_nSteamNetworkingSend_Reliable
            .MatchForward(
                useEnd: false,
                new CodeMatch(OpCodes.Ldstr),
                new CodeMatch(OpCodes.Ldloc_3),
                new CodeMatch(OpCodes.Box),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Call))
            .Advance(offset: 1)
            .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldloc_3))
            .SetInstructionAndAdvance(Transpilers.EmitDelegate<Action<string, ZSteamSocket, EResult>>(
                (error, socket, result) => {
                  switch (result) {
                    case EResult.k_EResultOK:
                    case EResult.k_EResultRateLimitExceeded:
                    case EResult.k_EResultNoConnection:
                      return;

                    default:
                      ZLog.Log($"{socket.m_peerID.GetSteamID64()}: {result}");
                      return;
                  }
                }
              ))
            .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Nop))
            .InstructionEnumeration();
      }
    }
  }
}