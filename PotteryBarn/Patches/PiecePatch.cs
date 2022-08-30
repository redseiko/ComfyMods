using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

using UnityEngine;

using static PotteryBarn.PotteryBarn;

namespace PotteryBarn.Patches {
  [HarmonyPatch(typeof(Piece))]
  public class PiecePatch {

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Piece), "DropResources")]
    public static void DropResourcePrefix(Piece __instance) {
      // Should not need to check against cultivator creator shop items here because they do not pass the Player.CanRemovePiece check
      if(Requirements.hammerCreatorShopItems.Keys.Contains(__instance.m_description)) {
        isDropTableDisabled = true;
      }

      log($"Piece description {__instance.m_description}");
      foreach (Piece.Requirement requirement in __instance.m_resources) {
        GameObject gameObject = requirement.m_resItem.gameObject;
        if(gameObject != null) {
          log($"Dropping {gameObject.name}");
        }
      }
    }
  }
  

    [HarmonyPatch(typeof(DropOnDestroyed))]
  public  class DropOnDestroyedPatch {

    [HarmonyPrefix]
    [HarmonyPatch(typeof(DropOnDestroyed), "OnDestroyed")]
    public static bool OnDestroyedPrefix(DropOnDestroyed __instance) {
      List<GameObject> dropList = __instance.m_dropWhenDestroyed.GetDropList();
      if (isDropTableDisabled) {
        isDropTableDisabled = false;
        return false;
      }
      log($"Item destroyed not player made from Pottery barn. Using normal drop table. Dropping items:");
      for (int i = 0; i < dropList.Count; i++) {
        log($"{dropList[i].name}");
      }
      return true;
    }
  }
}
