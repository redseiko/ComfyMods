using System.Collections.Generic;
using System.Linq;

using HarmonyLib;

using UnityEngine;

using static PotteryBarn.PotteryBarn;

namespace PotteryBarn {
  [HarmonyPatch(typeof(Piece))]
  static class PiecePatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Piece.DropResources))]
    public static bool DropResourcePrefix(Piece __instance) {
      // Should not need to check against cultivator creator shop items here because they do not pass the
      // Player.CanRemovePiece check.
      if (Requirements.hammerCreatorShopItems.Keys.Contains(__instance.m_description)) {
        if (__instance.IsCreator()) {
          IsDropTableDisabled = true;
          return true;
        }
        return false;
      }
      return true;
    }
  }

  [HarmonyPatch(typeof(DropOnDestroyed))]
  static class DropOnDestroyedPatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(DropOnDestroyed.OnDestroyed))]
    public static bool OnDestroyedPrefix(DropOnDestroyed __instance) {
      if (IsDropTableDisabled) {
        IsDropTableDisabled = false;
        return false;
      }
      return true;
    }
  }
}
