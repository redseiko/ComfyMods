namespace Insightful;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Hud))]
static class HudPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.UpdateCrosshair))]
  static void UpdateCrosshairPostfix(Hud __instance, Player player) {
    if (IsModEnabled.Value && InscriptionManager.HasInscription(player.m_hovering)) {
      __instance.m_hoverName.Append(
          $"[<color=yellow><b>{ReadHiddenTextShortcut.Value}</b></color>] Read Inscription");
    }
  }
}
