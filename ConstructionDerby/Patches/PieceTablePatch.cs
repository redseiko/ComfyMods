namespace ConstructionDerby;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(PieceTable))]
static class PieceTablePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(PieceTable.GetSelectedPrefab))]
  static void GetSelectedPrefabPostfix(ref GameObject __result) {
    if (IsModEnabled.Value && DerbyManager.HasCurrentGame()) {
      __result = DerbyManager.CurrentGame.CurrentPiece.gameObject;
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(PieceTable.GetSelectedPiece))]
  static void GetSelectedPiecePostfix(ref Piece __result) {
    if (IsModEnabled.Value && DerbyManager.HasCurrentGame()) {
      __result = DerbyManager.CurrentGame.CurrentPiece;
    }
  }
}
