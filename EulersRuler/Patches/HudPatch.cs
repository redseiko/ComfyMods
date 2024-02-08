namespace EulersRuler;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Hud))]
static class HudPatch {
  static GameObject _pieceHealthRoot;
  static GuiBar _pieceHealthBar;

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.Awake))]
  static void AwakePostfix(ref Hud __instance) {
    if (IsModEnabled.Value) {
      PanelUtils.CreatePanels(__instance);
    }

    _pieceHealthRoot = __instance.m_pieceHealthRoot.gameObject;
    _pieceHealthBar = __instance.m_pieceHealthBar;

    __instance.StartCoroutine(PanelUtils.UpdatePropertiesCoroutine());
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.UpdateCrosshair))]
  static void UpdateCrosshairPostfix(Hud __instance) {
    if (IsModEnabled.Value && _pieceHealthRoot.activeSelf) {
      if (ShowHoverPieceHealthBar.Value) {
        _pieceHealthBar.SetColor(PanelUtils.HealthPercentGradient.Evaluate(_pieceHealthBar.m_value));
      } else {
        _pieceHealthRoot.SetActive(false);
      }
    }
  }
}
