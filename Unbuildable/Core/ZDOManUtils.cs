namespace Unbuildable;

using System.Collections.Generic;

public static class ZDOManUtils {
  public static readonly HashSet<int> PrefabsToDestroy = [];

  public static void SetPrefabsToDestroy(IEnumerable<string> prefabs) {
    PrefabsToDestroy.Clear();

    foreach (string prefab in prefabs) {
      PrefabsToDestroy.Add(prefab.GetStableHashCode());
    }

    Unbuildable.LogInfo($"Watching for {PrefabsToDestroy.Count} prefabs to destroy on placement.");
  }

  public static bool DestroyZDOsDelegate(bool deadZDOsContainsKey, ZDO zdo) {
    if (!deadZDOsContainsKey && PrefabsToDestroy.Count > 0 && PrefabsToDestroy.Contains(zdo.m_prefab)) {
      ZRoutedRpc.s_instance.InvokeRoutedRPC(zdo.GetOwner(), zdo.m_uid, "RPC_Remove");
    }

    return deadZDOsContainsKey;
  }
}
