namespace ComfyLib;

using System.Collections.Generic;

public static class ZDOExtensions {
  public static bool TryGetLong(this ZDO zdo, int hash, out long value) {
    if (ZDOExtraData.s_longs.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, long> values)
        && values.TryGetValue(hash, out value)) {
      return true;
    }

    value = default;
    return false;
  }

  public static bool HasLong(this ZDO zdo, int hash) {
    return ZDOExtraData.s_longs.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, long> values)
        && values.ContainsKey(hash);
  }

  public static bool TryGetZDOID(this ZDO zdo, int userIdHash, int idHash, out ZDOID value) {
    if (ZDOExtraData.s_longs.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, long> values)
        && values.TryGetValue(userIdHash, out long userId)
        && values.TryGetValue(idHash, out long id)) {
      value = new(userId, (uint) id);
      return true;
    }

    value = ZDOID.None;
    return false;
  }

  public static bool HasZDOID(this ZDO zdo, int userIdHash, int idHash) {
    return ZDOExtraData.s_longs.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, long> values)
        && values.ContainsKey(userIdHash)
        && values.ContainsKey(idHash);
  }

  public static void SetZDOID(this ZDO zdo, int userIdHash, int idHash, ZDOID zdoid) {
    zdo.Set(userIdHash, zdoid.UserID);
    zdo.Set(idHash, zdoid.ID);
  }
}
