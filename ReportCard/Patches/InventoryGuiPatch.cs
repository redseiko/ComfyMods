namespace ReportCard;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(InventoryGui))]
static class InventoryGuiPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(InventoryGui.OnDestroy))]
  static void OnDestroyPrefix() {
    if (IsModEnabled.Value) {
      PlayerStatsController.DestroyStatsPanel();
    }
  }

  public static readonly int VisibleId = Animator.StringToHash("visible");

  [HarmonyPrefix]
  [HarmonyPatch(nameof(InventoryGui.Hide))]
  static void HidePrefix(InventoryGui __instance) {
    if (IsModEnabled.Value && __instance.m_animator.GetBool(VisibleId)) {
      PlayerStatsController.HideStatsPanel();
    }
  }
}
