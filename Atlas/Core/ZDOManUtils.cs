namespace Atlas;

using System;

using ComfyLib;

using static PluginConfig;

public static class ZDOManUtils {
  public static void SetTimeCreatedDelegate(ZDO zdo) {
    if (!zdo.TryGetLong(Atlas.TimeCreatedHashCode, out _)) {
      zdo.Set(Atlas.TimeCreatedHashCode, (long) (ZNet.m_instance.m_netTime * TimeSpan.TicksPerSecond));
      zdo.Set(Atlas.EpochTimeCreatedHashCode, DateTimeOffset.Now.ToUnixTimeSeconds());
    }

    if (!zdo.TryGetZDOID(Atlas.OriginalUidHashPair, out _)) {
      zdo.Set(Atlas.OriginalUidHashPair, zdo.m_uid);
    }
  }

  public static readonly int CatapultHashCode = "Catapult".GetStableHashCode();

  public static bool DestroyZDOsDelegate(bool deadZDOsContainsKey, ZDO zdo) {
    if (!deadZDOsContainsKey && zdo.m_prefab == CatapultHashCode) {
      ZRoutedRpc.s_instance.InvokeRoutedRPC(zdo.GetOwner(), zdo.m_uid, "RPC_Remove");
    }

    return deadZDOsContainsKey;
  }

  public static int FireplaceZDOsModified = 0;
  public static int WearNTearZDOsModified = 0;

  public static void SetTimeCreatedFields(ZDO zdo) {
    if (!zdo.TryGetLong(Atlas.TimeCreatedHashCode, out _)) {
      ZDOExtraData.Set(zdo.m_uid, Atlas.TimeCreatedHashCode, TimeSpan.TicksPerSecond);
      ZDOExtraData.Set(zdo.m_uid, Atlas.EpochTimeCreatedHashCode, 1L);
    }
  }

  public static void SetAshlandsCustomFields(ZDO zdo) {
    if (!ZDOExtraData.GetLong(zdo.m_uid, ZDOVars.s_creator, out long _)) {
      return;
    }

    if (PrefabUtils.FireplacePrefabs.Contains(zdo.m_prefab)
        && WorldGenerator.m_instance.GetBiome(zdo.m_position) == Heightmap.Biome.AshLands) {
      CustomFieldUtils.SetFireplaceFields(zdo, igniteChance: 0f, igniteSpread: 0);
      FireplaceZDOsModified++;
    }

    if (PrefabUtils.WearNTearPrefabs.Contains(zdo.m_prefab)
        && WorldGenerator.m_instance.GetBiome(zdo.m_position) == Heightmap.Biome.AshLands) {
      CustomFieldUtils.SetWearNTearFields(zdo, burnable: false);
      WearNTearZDOsModified++;
    }
  }

  public static void LogModifiedZDOs() {
    PluginLogger.LogInfo($"Modified {FireplaceZDOsModified} Fireplace ZDOs.");
    PluginLogger.LogInfo($"Modified {WearNTearZDOsModified} WearNTear ZDOs.");
  }
}
