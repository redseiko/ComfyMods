namespace GetOffMyLawn;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(PrivateArea))]
static class PrivateAreaPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(PrivateArea.Interact))]
  static void InteractPostfix(PrivateArea __instance) {
    if (IsModEnabled.Value
        && RepairPiecesOnWardActivation.Value
        && __instance
        && __instance.IsEnabled()
        && __instance.m_piece.IsCreator()) {
      PieceUtils.RepairPiecesInRadius(__instance.transform.position, __instance.m_radius);
    }
  }
}
