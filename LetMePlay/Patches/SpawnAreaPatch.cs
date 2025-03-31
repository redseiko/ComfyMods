namespace LetMePlay;

using System;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(SpawnArea))]
static class SpawnAreaPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(SpawnArea.Awake))]
  static void AwakePostfix(SpawnArea __instance) {
    if (IsModEnabled.Value && __instance.m_prefabs != null) {
      __instance.m_prefabs.RemoveAll(_hasInvalidPrefab);
    }
  }

  static readonly Predicate<SpawnArea.SpawnData> _hasInvalidPrefab = HasInvalidPrefab;

  static bool HasInvalidPrefab(SpawnArea.SpawnData spawnData) {
    return !spawnData.m_prefab;
  }
}
