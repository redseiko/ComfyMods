namespace Unbuildable;

using System.Collections.Generic;

public static class ZDOManUtils {
  public static List<int> PrefabsToRemove { get; } = [];
  public static List<int> PrefabsToDestroy { get; } = [];

  public static void SetPrefabsToRemove(IEnumerable<int> prefabHashes) {
    PrefabsToRemove.Clear();
    PrefabsToRemove.AddRange(prefabHashes);

    Unbuildable.LogInfo($"Watching for {PrefabsToRemove.Count} prefabs to remove on placement.");
  }

  public static void SetPrefabsToDestroy(IEnumerable<int> prefabHashes) {
    PrefabsToDestroy.Clear();
    PrefabsToDestroy.AddRange(prefabHashes);

    Unbuildable.LogInfo($"Watching for {PrefabsToDestroy.Count} prefabs to destroy on creation.");
  }

  public static bool DestroyZDOsDelegate(bool deadZDOsContainsKey, ZDO zdo) {
    if (deadZDOsContainsKey) {
      return true;
    }

    if (PrefabsToDestroy.Count > 0 && PrefabsToDestroy.Contains(zdo.m_prefab)) {
      return true;
    }

    if (PrefabsToRemove.Count > 0 && PrefabsToRemove.Contains(zdo.m_prefab)) {
      ZRoutedRpc.s_instance.InvokeRoutedRPC(zdo.GetOwner(), zdo.m_uid, "RPC_Remove", false);
    }

    return false;
  }
}
