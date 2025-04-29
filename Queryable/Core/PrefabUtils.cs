namespace Queryable;

using System.Collections.Generic;
using System.Linq;

using ComfyLib;

using UnityEngine;

public static class PrefabUtils {
  public static bool TryGetPrefabCache(out Dictionary<int, GameObject> prefabCache) {
    prefabCache = default;

    if (!TryGetNetScene(out ZNetScene netScene)) {
      ComfyLogger.LogError($"Could not find a valid ZNetScene!");
      return false;
    }

    if (netScene.m_namedPrefabs.Count > 0) {
      prefabCache = new(netScene.m_namedPrefabs);
      return true;
    }

    prefabCache = new(capacity: netScene.m_prefabs.Count);

    foreach (GameObject prefab in netScene.m_prefabs) {
      prefabCache[prefab.name.GetStableHashCode()] = prefab;
    }

    return true;
  }

  static bool TryGetNetScene(out ZNetScene netScene) {
    netScene = ZNetScene.s_instance;

    if (!netScene) {
      netScene = Resources.FindObjectsOfTypeAll<ZNetScene>().FirstOrDefault();
    }

    return netScene;
  }

  public static readonly Dictionary<int, ArmorStand> ArmorStandPrefabs = [];
  public static readonly Dictionary<int, ItemStand> ItemStandPrefabs = [];
  public static readonly Dictionary<int, ItemDrop> ItemDropPrefabs = [];

  public static readonly List<ItemSlot> ArmorStandItemSlots = [];

  public static void CacheArmorStandSlots(int slotCount) {
    for (int i = ArmorStandItemSlots.Count; i < slotCount; i++) {
      ArmorStandItemSlots.Add(new(i));
    }
  }

  public static void ProcessPrefabCache(Dictionary<int, GameObject> prefabCache) {
    ArmorStandPrefabs.Clear();
    ItemStandPrefabs.Clear();
    ItemDropPrefabs.Clear();

    foreach (KeyValuePair<int, GameObject> pair in prefabCache) {
      int prefabHash = pair.Key;
      GameObject prefab = pair.Value;

      if (prefab.TryGetComponent(out ArmorStand armorStand)) {
        ArmorStandPrefabs[prefabHash] = armorStand;
      }

      if (prefab.TryGetComponent(out ItemStand itemStand)) {
        ItemStandPrefabs[prefabHash] = itemStand;
      }

      if (prefab.TryGetComponent(out ItemDrop itemDrop)) {
        ItemDropPrefabs[prefabHash] = itemDrop;
      }
    }

    ComfyLogger.LogInfo(
        $"Processed prefab-cache.\n"
            + $"Cached prefab-hashes by type:\n"
            + $"  * ArmorStand: {ArmorStandPrefabs.Count}\n"
            + $"  * ItemStand: {ItemStandPrefabs.Count}\n"
            + $"  * ItemDrop: {ItemDropPrefabs.Count}");
  }

  public static bool HasArmorStandComponent(ZDO zdo, out int slotCount) {
    if (ArmorStandPrefabs.TryGetValue(zdo.m_prefab, out ArmorStand armorStand)) {
      slotCount = armorStand.m_slots.Count;
      return true;
    }

    slotCount = 0;
    return false;
  }

  public static bool HasItemStandComponent(ZDO zdo) {
    return ItemStandPrefabs.ContainsKey(zdo.m_prefab);
  }

  public static bool HasItemDropComponent(ZDO zdo) {
    return ItemDropPrefabs.ContainsKey(zdo.m_prefab);
  }
}
