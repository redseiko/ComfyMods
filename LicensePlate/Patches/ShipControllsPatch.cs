namespace LicensePlate;

using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ShipControlls))]
static class ShipControllsPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ShipControlls.Awake))]
  static void AwakePostfix(ref ShipControlls __instance) {
    if (IsModEnabled.Value && ShowShipNames.Value) {
      __instance.gameObject.AddComponent<ShipName>();
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ShipControlls.Interact))]
  static IEnumerable<CodeInstruction> InteractTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(
                OpCodes.Callvirt, AccessTools.Method(typeof(Character), nameof(Character.GetStandingOnShip))),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ShipControlls), nameof(ShipControlls.m_ship))),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UnityEngine.Object), "op_Inequality")))
        .Advance(offset: 5)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldarg_3),
            Transpilers.EmitDelegate(StandingOnShipInequalityDelegate))
        .InstructionEnumeration();
  }

  static bool StandingOnShipInequalityDelegate(bool isNotEqual, ShipControlls shipControls, bool alt) {
    if (!isNotEqual
        && alt
        && IsModEnabled.Value
        && ShowShipNames.Value
        && PrivateArea.CheckAccess(shipControls.transform.position)
        && shipControls.m_nview
        && shipControls.m_nview.IsValid()
        && shipControls.m_nview.IsOwner()
        && shipControls.TryGetComponent(out ShipName shipName)) {
      TextInput.m_instance.RequestText(shipName, "$hud_rename", 64);
      return true;
    }

    return isNotEqual;
  }

  static readonly Lazy<string> _renameText =
      new(() =>
          Localization.m_instance.Localize("\n[<color=yellow><b>$KEY_AltPlace + $KEY_Use</b></color>] $hud_rename"));

  [HarmonyPostfix]
  [HarmonyPatch(nameof(ShipControlls.GetHoverText))]
  static void GetHoverTextPostfix(ref ShipControlls __instance, ref string __result) {
    if (IsModEnabled.Value
        && ShowShipNames.Value
        && __instance.m_nview
        && __instance.m_nview.IsValid()
        && Player.m_localPlayer
        && __instance.InUseDistance(Player.m_localPlayer)) {
      __result += _renameText.Value;
    }
  }
}
