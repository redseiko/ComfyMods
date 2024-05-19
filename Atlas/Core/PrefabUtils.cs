namespace Atlas;

using System.Collections.Generic;

using UnityEngine;

public static class PrefabUtils {
  public static readonly HashSet<int> FireplacePrefabs = [];
  public static readonly HashSet<int> WearNTearPrefabs = [];

  public static void RegisterPrefabs(Dictionary<int, GameObject> prefabsByHashCode) {
    foreach (KeyValuePair<int, GameObject> pair in prefabsByHashCode) {
      if (pair.Value.TryGetComponent(out Fireplace fireplace) && fireplace.m_igniteChance > 0f) {
        FireplacePrefabs.Add(pair.Key);
      }

      if (pair.Value.TryGetComponent(out WearNTear wearNTear) && wearNTear.m_burnable) {
        WearNTearPrefabs.Add(pair.Key);
      }
    }

    PluginLogger.LogInfo($"Registered {FireplacePrefabs.Count} Fireplace prefabs.");
    PluginLogger.LogInfo($"Registered {WearNTearPrefabs.Count} WearNTear prefabs.");
  }
}
