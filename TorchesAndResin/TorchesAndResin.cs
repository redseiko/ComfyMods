using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Reflection;

namespace TorchesAndResin {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class TorchesAndResin : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.torchesandresin";
    public const string PluginName = "Torches and Resin";
    public const string PluginVersion = "1.0.0";

    private const float _torchStartingFuel = 10000f;

    private static readonly int _fuelHashCode = "fuel".GetStableHashCode();
    private static readonly string[] _eligibleTorchItemNames = {
      "piece_groundtorch_wood", // standing wood torch
      "piece_groundtorch", // standing iron torch
      "piece_walltorch", // sconce torch
      "piece_brazierceiling01" // brazier
    };

    private static ConfigEntry<bool> _isModEnabled;

    private Harmony _harmony;

    private void Awake() {
      _isModEnabled = Config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    [HarmonyPatch(typeof(Fireplace))]
    class FireplacePatch {
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
        __instance.m_nview.GetZDO().Set(_fuelHashCode, __instance.m_startFuel);
      }
    }
  }
}
