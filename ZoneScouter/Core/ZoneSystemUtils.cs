namespace ZoneScouter;

using System;
using System.Reflection;

using HarmonyLib;

using UnityEngine;

public static class ZoneSystemUtils {
  static Func<ZoneSystem, Vector3, Vector2i> _getZoneFunc;
  static Func<Vector3, Vector2i> _getZoneStaticFunc;
  static Func<ZoneSystem, Vector2i, Vector3> _getZonePosFunc;
  static Func<Vector2i, Vector3> _getZonePosStaticFunc;

  static MethodInfo _getZoneMethod;
  static MethodInfo _getZonePosMethod;

  public static Vector2i GetZone(Vector3 point) {
    if (_getZoneMethod.IsStatic) {
      return _getZoneStaticFunc(point);
    }

    return _getZoneFunc(ZoneSystem.m_instance, point);
  }

  public static Vector3 GetZonePos(Vector2i zoneId) {
    if (_getZonePosMethod.IsStatic) {
      return _getZonePosStaticFunc(zoneId);
    }

    return _getZonePosFunc(ZoneSystem.m_instance, zoneId);
  }

  public static void SetupUtils() {
    _getZoneMethod = AccessTools.Method(typeof(ZoneSystem), nameof(ZoneSystem.GetZone), [typeof(Vector3)]);

    if (_getZoneMethod.IsStatic) {
      _getZoneStaticFunc =
          (Func<Vector3, Vector2i>)
              Delegate.CreateDelegate(typeof(Func<Vector3, Vector2i>), _getZoneMethod);
    } else {
      _getZoneFunc =
          (Func<ZoneSystem, Vector3, Vector2i>)
              Delegate.CreateDelegate(typeof(Func<ZoneSystem, Vector3, Vector2i>), _getZoneMethod);
    }

    _getZonePosMethod =
        AccessTools.Method(typeof(ZoneSystem), nameof(ZoneSystem.GetZonePos), [typeof(Vector2i)]);

    if (_getZonePosMethod.IsStatic) {
      _getZonePosStaticFunc =
          (Func<Vector2i, Vector3>)
              Delegate.CreateDelegate(typeof(Func<Vector2i, Vector3>), _getZonePosMethod);
    } else {
      _getZonePosFunc =
          (Func<ZoneSystem, Vector2i, Vector3>)
              Delegate.CreateDelegate(typeof(Func<ZoneSystem, Vector2i, Vector3>), _getZonePosMethod);
    }
  }
}
