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

  public static bool TryGetZDOID(this ZDO zdo, KeyValuePair<int, int> hashPair, out ZDOID value) {
    if (ZDOExtraData.s_longs.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, long> values)
        && values.TryGetValue(hashPair.Key, out long userIdPart)
        && values.TryGetValue(hashPair.Value, out long idPart)) {
      value = new(userIdPart, (uint) idPart);
      return true;
    }

    value = ZDOID.None;
    return false;
  }
}
