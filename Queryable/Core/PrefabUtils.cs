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
}
