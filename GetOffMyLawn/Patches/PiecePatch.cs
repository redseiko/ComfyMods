namespace GetOffMyLawn;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Piece))]
static class PiecePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Piece.SetCreator))]
  static void SetCreatorPostfix(Piece __instance) {
    if (IsModEnabled.Value && __instance && __instance.m_nview && !__instance.TryGetComponent(out Plant _)) {
      __instance.m_nview.GetZDO().Set(ZDOVars.s_health, TargetPieceHealth.Value);
    }
  }
}
