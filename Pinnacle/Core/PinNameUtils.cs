namespace Pinnacle;

using System;
using System.Collections.Generic;

using UnityEngine;

public static class PinNameUtils {
  static readonly string[] _pinTypeNames = Enum.GetNames(typeof(Minimap.PinType));
  static readonly Dictionary<Sprite, string> _locationSpriteNames = [];

  public static void Initialize(Minimap minimap) {
    _locationSpriteNames.Clear();

    if (!minimap || minimap.m_locationIcons == null) {
      return;
    }

    foreach (Minimap.LocationSpriteData locationSpriteData in minimap.m_locationIcons) {
      if (locationSpriteData.m_icon) {
        _locationSpriteNames[locationSpriteData.m_icon] = locationSpriteData.m_name;
      }
    }
  }

  public static void Reset() {
    _locationSpriteNames.Clear();
  }

  public static string GetFallbackPinName(Minimap.PinData pin) {
    if (pin.m_type == Minimap.PinType.None && pin.m_icon) {
      return _locationSpriteNames.TryGetValue(pin.m_icon, out string locationName)
          ? locationName
          : pin.m_icon.name;
    }

    int typeIndex = (int) pin.m_type;

    return (typeIndex >= 0 && typeIndex < _pinTypeNames.Length)
        ? _pinTypeNames[typeIndex]
        : pin.m_type.ToString();
  }
}
