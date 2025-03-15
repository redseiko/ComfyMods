namespace ColorfulWards;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(PrivateArea))]
static class PrivateAreaPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(PrivateArea.IsInside))]
  static IEnumerable<CodeInstruction> IsInsideTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.DistanceXZ))))
        .ThrowIfInvalid($"Could not patch PrivateArea.IsInside()! (distance-xz)")
        .SetOperandAndAdvance(AccessTools.Method(typeof(PrivateAreaPatch), nameof(DistanceDelegate)))
        .InstructionEnumeration();
  }

  static float DistanceDelegate(Vector3 point0, Vector3 point1) {
    return
        IsModEnabled.Value && UseRadiusForVerticalCheck.Value
            ? Vector3.Distance(point0, point1)
            : Utils.DistanceXZ(point0, point1);
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(PrivateArea.Awake))]
  static void AwakePostfix(PrivateArea __instance) {
    if (IsModEnabled.Value && __instance) {
      AddPrivateAreaColor(__instance.m_nview);
    }
  }

  static void AddPrivateAreaColor(ZNetView netView) {
    if (netView && netView.IsValid() && !netView.TryGetComponent(out PrivateAreaColor _)) {
      netView.gameObject.AddComponent<PrivateAreaColor>();
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(PrivateArea.UpdateStatus))]
  static void UpdateStatusPostfix(PrivateArea __instance) {
    if (IsModEnabled.Value && __instance.TryGetComponent(out PrivateAreaColor privateAreaColor)) {
      privateAreaColor.UpdateColors();
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(PrivateArea.GetHoverText))]
  static void GetHoverTextPostfix(PrivateArea __instance, ref string __result) {
    if (!IsModEnabled.Value || !ShowChangeColorHoverText.Value || !__instance.m_piece.IsCreator()) {
      return;
    }

    __result =
        string.Format(
            "{0}\n[<color={1}>{2}</color>] Change ward color to: <color=#{3}>#{3}</color>",
            __result,
            "#FFA726",
            ChangeWardColorShortcut.Value,
            TargetWardColor.Value.GetColorHtmlString());
  }
}
