namespace PotteryBarn;

using System.Collections.Generic;
using System.Linq;

using HarmonyLib;

using UnityEngine;

using static DvergrPieces;

[HarmonyPatch(typeof(Piece))]
static class PiecePatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(Piece.DropResources))]
  static bool DropResourcePrefix(Piece __instance) {
    // Should not need to check against cultivator creator shop items here because they do not pass the
    // Player.CanRemovePiece check.
    if (Requirements.HammerCreatorShopItems.Keys.Contains(__instance.m_description)) {
      if (__instance.TryGetComponent(out Container container)) {
        DropContainerContents(container);
      }

      if (__instance.IsCreator()) {
        PotteryBarn.IsDropTableDisabled = true;
        return true;
      }

      return false;
    }

    if (DvergrPrefabs.Keys.Contains(__instance.m_description) && !__instance.IsPlacedByPlayer()) {
      DropDefaultResources(__instance);
      return false;
    }

    return true;
  }

  static void DropDefaultResources(Piece piece) {
    foreach (KeyValuePair<string, int> req in DvergrPrefabDefaultDrops[piece.m_description]) {
      Container container = null;
      GameObject gameObject = ZNetScene.instance.GetPrefab(req.Key);
      int amount = req.Value;

      if (piece.m_destroyedLootPrefab) {
        while (amount > 0) {
          ItemDrop.ItemData itemData = gameObject.GetComponent<ItemDrop>().m_itemData.Clone();
          itemData.m_dropPrefab = gameObject;
          itemData.m_stack = Mathf.Min(amount, itemData.m_shared.m_maxStackSize);
          amount -= itemData.m_stack;

          if (container == null || !container.GetInventory().HaveEmptySlot()) {
            container =
                    Object.Instantiate(
                        piece.m_destroyedLootPrefab,
                        piece.transform.position + Vector3.up,
                        Quaternion.identity)
                .GetComponent<Container>();
          }

          container.GetInventory().AddItem(itemData);
        }
      } else {
        while (amount > 0) {
          ItemDrop component =
                  Object.Instantiate(
                      gameObject,
                      piece.transform.position + Vector3.up,
                      Quaternion.identity)
              .GetComponent<ItemDrop>();

          component.SetStack(Mathf.Min(amount, component.m_itemData.m_shared.m_maxStackSize));
          amount -= component.m_itemData.m_stack;
        }
      }
    }
  }

  static void DropContainerContents(Container container) {
    container.DropAllItems();
  }
}
