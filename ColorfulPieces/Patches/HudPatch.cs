namespace ColorfulPieces;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Hud))]
static class HudPatch {
  public static readonly string HoverNameTextTemplate =
    "{0}{1}"
        + "<size={8}>"
        + "[<color={2}>{3}</color>] Change color: <color=#{4}>#{4}</color> (<color=#{4}>{5}</color>)\n"
        + "[<color={6}>{7}</color>] Clear color\n"
        + "</size>";

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.UpdateCrosshair))]
  static void UpdateCrosshairPostfix(Hud __instance, Player player) {
    if (!IsModEnabled.Value
        || !ShowChangeRemoveColorPrompt.Value
        || !player.m_hovering
        || !player.m_hovering.TryGetComponentInParent(out WearNTear wearNTear)
        || !wearNTear
        || !wearNTear.m_nview
        || !wearNTear.m_nview.IsValid()) {
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
            ColorPromptFontSize.Value);
  }
}
