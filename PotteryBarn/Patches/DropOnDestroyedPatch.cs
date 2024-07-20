namespace PotteryBarn;

using HarmonyLib;

[HarmonyPatch(typeof(DropOnDestroyed))]
static class DropOnDestroyedPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(DropOnDestroyed.OnDestroyed))]
  static bool OnDestroyedPrefix(DropOnDestroyed __instance) {
    if (__instance.TryGetComponent(out Piece piece)
        && piece.IsPlacedByPlayer()
        && PotteryManager.IsVanillaPiece(piece)) {
      return false;
    }

    if (PotteryManager.IsDropTableDisabled) {
      PotteryManager.IsDropTableDisabled = false;
      return false;
    }

    return true;
  }
}
