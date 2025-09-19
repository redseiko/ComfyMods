namespace GetOffMyLawn;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Piece))]
static class PiecePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Piece.SetCreator))]
  static void SetCreatorPostfix(Piece __instance) {
    if (IsModEnabled.Value
        && __instance
        && PieceUtils.TryGetValidOwnedNetView(__instance, out ZNetView netView)
        && PieceUtils.IsEligiblePiece(__instance)) {
      netView.m_zdo.Set(ZDOVars.s_health, TargetPieceHealth.Value);
    }
  }
}
