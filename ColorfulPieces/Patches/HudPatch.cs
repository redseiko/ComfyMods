using HarmonyLib;

using UnityEngine;

using static ColorfulPieces.PluginConfig;

namespace ColorfulPieces {
  [HarmonyPatch(typeof(Hud))]
  static class HudPatch {
    static readonly string HoverNameTextTemplate =
      "{0}{1}"
          + "<size={9}>"
          + "[<color={2}>{3}</color>] Set piece color: <color=#{4}>#{4}</color> (<color=#{4}>{5}</color>)\n"
          + "[<color={6}>{7}</color>] Clear piece color\n"
          + "[<color={6}>{8}</color>] Copy piece color\n"
          + "</size>";

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Hud.UpdateCrosshair))]
    static void UpdateCrosshairPostfix(ref Hud __instance, ref Player player) {
      if (!IsModEnabled.Value || !ShowChangeRemoveColorPrompt.Value || !Player.m_localPlayer.Ref()?.m_hovering) {
        return;
      }

      WearNTear wearNTear = player.m_hovering.GetComponentInParent<WearNTear>();

      if (!wearNTear.Ref()?.m_nview || !wearNTear.m_nview.IsValid()) {
        return;
      }

      __instance.m_hoverName.text =
          string.Format(
              HoverNameTextTemplate,
              __instance.m_hoverName.text,
              __instance.m_hoverName.text.Length > 0 ? "\n" : string.Empty,
              "#FFA726",
              ChangePieceColorShortcut.Value,
              ColorUtility.ToHtmlStringRGB(TargetPieceColor.Value),
              TargetPieceEmissionColorFactor.Value.ToString("N2"),
              "#EF5350",
              ClearPieceColorShortcut.Value,
              CopyPieceColorShortcut.Value,
              ColorPromptFontSize.Value);
    }
  }
}
