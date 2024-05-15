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

  public static readonly int CatapultHashCode = "Catapult".GetStableHashCode();

  public static bool DestroyZDOsDelegate(bool deadZDOsContainsKey, ZDO zdo) {
    if (!deadZDOsContainsKey && zdo.m_prefab == CatapultHashCode) {
      ZRoutedRpc.s_instance.InvokeRoutedRPC(zdo.GetOwner(), zdo.m_uid, "RPC_Remove");
    }

    return deadZDOsContainsKey;
  }
}
