namespace PotteryBarn;

using HarmonyLib;

[HarmonyPatch(typeof(Piece))]
static class PiecePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Piece.Awake))]
  static void AwakePostfix(Piece __instance) {
    if (PotteryManager.IsPlacingPiece) {
      return;
    }

    PotteryManager.SetupPiece(__instance);
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Piece.DropResources))]
  static void DropResourcesPrefix(Piece __instance) {
    if (__instance.TryGetComponent(out Container container)) {
      container.DropAllItems();
    }
  }
}
