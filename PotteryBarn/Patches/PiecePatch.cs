namespace PotteryBarn;

using System.Collections.Generic;

using HarmonyLib;

using UnityEngine;

using static DvergrPieces;

[HarmonyPatch(typeof(Piece))]
static class PiecePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Piece.Awake))]
  static void AwakePostfix(Piece __instance) {
    if (__instance.m_canBeRemoved && !PotteryManager.CanBeRemoved(__instance)) {
      __instance.m_canBeRemoved = false;
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Piece.DropResources))]
  static bool DropResourcePrefix(Piece __instance) {
    // Should not need to check against cultivator creator shop items here because they do not pass the
    // Player.CanRemovePiece check.
    if (PotteryManager.IsShopPiece(__instance)) {
      if (__instance.TryGetComponent(out Container container)) {
        DropContainerContents(container);
      }

      if (__instance.IsCreator()) {
        PotteryManager.IsDropTableDisabled = true;
        return true;
      }

      return false;
    }

    // TODO: remove me and alter Piece resources on Awake() instead.
    if (PotteryManager.IsDvergrPiece(__instance) && !__instance.IsPlacedByPlayer()) {
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
