using UnityEngine;

namespace ComfyLib {
  public static class ZDOExtensions {
    public static bool TryGetVector3(this ZDO zdo, int hash, out Vector3 value) {
      if (ZDOExtraData.s_vec3.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, Vector3> values)
          && values.TryGetValue(hash, out value)) {
        return true;
      }

      value = default;
      return false;
    }

    public static bool ClearFloat(this ZDO zdo, int hash) {
      if (ZDOExtraData.s_floats.Remove(zdo.m_uid, hash)) {
        zdo.IncreaseDataRevision();
        return true;
      }

      return false;
    }

    public static bool ClearVector3(this ZDO zdo, int hash) {
      if (ZDOExtraData.s_vec3.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, Vector3> values)
          && values.Remove(hash)) {
        ZLog.Log($"Removed Vec3:{hash} from {zdo.m_uid}");
        zdo.IncreaseDataRevision();
        return true;
      }

      return false;
    }
  }
}
