namespace ConstructionDerby;

using System.Collections.Generic;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.GetBuildPieces))]
  static void GetBuildPiecesPostfix(ref List<Piece> __result) {
    if (IsModEnabled.Value && DerbyManager.HasCurrentGame()) {
      __result.Clear();
      __result.Add(DerbyManager.CurrentGame.CurrentPiece);
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.PlacePiece))]
  static void PlacePiecePostfix() {
    if (IsModEnabled.Value && DerbyManager.HasCurrentGame()) {
      DerbyManager.CurrentGame.SelectNextPiece();
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.CheckCanRemovePiece))]
  static void CheckCanRemovePiece(ref bool __result) {
    if (IsModEnabled.Value && DerbyManager.HasCurrentGame()) {
      __result = TestingCanRemovePiece.Value;
    }
  }
}
