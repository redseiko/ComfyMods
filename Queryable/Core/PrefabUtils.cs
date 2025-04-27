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

  static readonly HashSet<int> ArmorStandPrefabHashes = [];
  static readonly HashSet<int> ItemStandPrefabHashes = [];
  static readonly HashSet<int> ItemDropPrefabHashes = [];

  public static void ProcessPrefabCache(Dictionary<int, GameObject> prefabCache) {
    ArmorStandPrefabHashes.Clear();
    ItemStandPrefabHashes.Clear();
    ItemDropPrefabHashes.Clear();

    foreach (KeyValuePair<int, GameObject> pair in prefabCache) {
      int prefabHash = pair.Key;
      GameObject prefab = pair.Value;

      if (prefab.TryGetComponent(out ArmorStand _)) {
        ArmorStandPrefabHashes.Add(prefabHash);
      }

      if (prefab.TryGetComponent(out ItemStand _)) {
        ItemStandPrefabHashes.Add(prefabHash);
      }

      if (prefab.TryGetComponent(out ItemDrop _)) {
        ItemDropPrefabHashes.Add(prefabHash);
      }
    }

    ComfyLogger.LogInfo(
        $"Processed prefab-cache.\n"
            + $"Cached prefab-hashes by type:\n"
            + $"  * ArmorStand: {ArmorStandPrefabHashes.Count}\n"
            + $"  * ItemStand: {ItemStandPrefabHashes.Count}\n"
            + $"  * ItemDrop: {ItemDropPrefabHashes.Count}");
  }

  public static bool HasArmorStandComponent(ZDO zdo) {
    return ArmorStandPrefabHashes.Contains(zdo.m_prefab);
  }

  public static bool HasItemStandComponent(ZDO zdo) {
    return ItemStandPrefabHashes.Contains(zdo.m_prefab);
  }

  public static bool HasItemDropComponent(ZDO zdo) {
    return ItemDropPrefabHashes.Contains(zdo.m_prefab);
  }
}
