using System.Collections.Generic;

namespace PutMeDown {
  public static class AutoPickupController {
    static readonly HashSet<string> _itemsToIgnore = new();

    public static void SetItemsToIgnore(IEnumerable<string> itemsToIgnore) {
      _itemsToIgnore.Clear();
      _itemsToIgnore.UnionWith(itemsToIgnore);

      PutMeDown.LogInfo($"Now ignoring items: {string.Join(", ", _itemsToIgnore)}");
    }

    public static bool ShouldIgnoreItem(ItemDrop itemDrop) {
      return _itemsToIgnore.Contains(itemDrop.m_itemData.m_dropPrefab.name);
    }
  }
}
