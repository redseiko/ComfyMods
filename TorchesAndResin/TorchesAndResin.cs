using BepInEx;
using BepInEx.Configuration;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace TorchesAndResin {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class TorchesAndResin : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.torchesandresin";
    public const string PluginName = "TorchesAndResin";
    public const string PluginVersion = "1.2.0";

    const float _torchStartingFuel = 10000f;

    static readonly int _fuelHashCode = "fuel".GetStableHashCode();
    static readonly string[] _eligibleTorchItemNames = {
      "piece_groundtorch_wood", // standing wood torch
      "piece_groundtorch", // standing iron torch
      "piece_walltorch", // sconce torch
      "piece_brazierceiling01" // brazier
    };

    static ConfigEntry<bool> _isModEnabled;
    Harmony _harmony;

    public void Awake() {
      _isModEnabled = Config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginVersion);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    [HarmonyPatch(typeof(Fireplace))]
    class FireplacePatch {
      [HarmonyTranspiler]
      [HarmonyPatch(nameof(Fireplace.Awake))]
      static IEnumerable<CodeInstruction> AwakeTranspiler(IEnumerable<CodeInstruction> instructions) {
        return new CodeMatcher(instructions)
            .MatchForward(
                useEnd: false,
                new CodeMatch(OpCodes.Ldstr, "UpdateFireplace"),
                new CodeMatch(OpCodes.Ldc_R4, 0f),
                new CodeMatch(OpCodes.Ldc_R4, 2f))
            .Advance(offset: 1)
            .SetOperandAndAdvance(0.5f)
            .InstructionEnumeration();
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(Fireplace.Awake))]
      static void AwakePostfix(ref Fireplace __instance) {
        if (!_isModEnabled.Value
            || !__instance.m_nview
            || !__instance.m_nview.IsValid()
            || !__instance.m_nview.IsOwner()
            || Array.IndexOf(_eligibleTorchItemNames, __instance.m_nview.GetPrefabName()) < 0) {
          return;
        }

        __instance.m_startFuel = _torchStartingFuel;
        __instance.m_nview.GetZDO().Set(_fuelHashCode, _torchStartingFuel);
      }
    }
  }
}
