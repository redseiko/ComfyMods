namespace PotteryBarn;

using System.Linq;

using HarmonyLib;

[HarmonyPatch(typeof(DropOnDestroyed))]
static class DropOnDestroyedPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(DropOnDestroyed.OnDestroyed))]
  static bool OnDestroyedPrefix(DropOnDestroyed __instance) {
    if (__instance.TryGetComponent(out Piece piece)
        && DvergrPieces.DvergrPrefabs.Keys.Contains(piece.m_description)
        && piece.IsPlacedByPlayer()) {
      return false;
    }

    if (PotteryManager.IsDropTableDisabled) {
      PotteryManager.IsDropTableDisabled = false;
      return false;
    }

    return true;
  }
}
