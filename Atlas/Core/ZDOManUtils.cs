namespace Atlas;

using System;

using ComfyLib;

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

  public static int FireplaceZDOsModified = 0;
  public static int WearNTearZDOsModified = 0;

  public static void SetTimeCreatedFields(ZDO zdo) {
    if (!zdo.TryGetLong(Atlas.TimeCreatedHashCode, out _)) {
      ZDOExtraData.Set(zdo.m_uid, Atlas.TimeCreatedHashCode, TimeSpan.TicksPerSecond);
      ZDOExtraData.Set(zdo.m_uid, Atlas.EpochTimeCreatedHashCode, 1L);
    }
  }
}
