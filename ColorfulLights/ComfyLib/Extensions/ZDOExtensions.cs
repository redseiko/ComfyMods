namespace ComfyLib;

using UnityEngine;

public static class ZDOExtensions {
  public static bool TryGetVector3(this ZDO zdo, int keyHashCode, out Vector3 value) {
    if (ZDOExtraData.s_vec3.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, Vector3> values)
        && values.TryGetValue(keyHashCode, out value)) {
      return true;
    }

    value = default;
    return false;
  }
}
