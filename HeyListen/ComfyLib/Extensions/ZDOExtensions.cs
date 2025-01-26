using UnityEngine;

namespace ComfyLib;

public static class ZDOExtensions {
  public static void Set(this ZDO zdo, int key, Color value) {
    if (ZDOExtraData.s_quats.InitAndSet(zdo.m_uid, key, new Quaternion(value.r, value.g, value.b, value.a))) {
      zdo.IncreaseDataRevision();
    }
  }

  public static Color GetColor(this ZDO zdo, int key, Color defaultValue) {
    if (ZDOExtraData.s_quats.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, Quaternion> values)
        && values.TryGetValue(key, out Quaternion value)) {
      return new(value.x, value.y, value.z, value.w);
    }

    return defaultValue;
  }

  public static bool TryGetColor(this ZDO zdo, int key, out Color result) {
    if (ZDOExtraData.s_quats.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, Quaternion> values)
        && values.TryGetValue(key, out Quaternion value)) {
      result = new(value.x, value.y, value.z, value.w);
      return true;
    }

    result = default;
    return false;
  }

  public static bool TryGetFloat(this ZDO zdo, int key, out float result) {
    if (ZDOExtraData.s_floats.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, float> values)
        && values.TryGetValue(key, out result)) {
      return true;
    }

    result = default;
    return false;
  }
}
