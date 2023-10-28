using System.Collections.Generic;

using ConfigurationManager;

using UnityEngine;

namespace Configula {
  public class Vector2SettingField {
    public FloatInputField XInput;
    public FloatInputField YInput;

    public Vector2SettingField(Vector2 value) {
      XInput = new("X");
      YInput = new("Y");
    }

    public void SetValue(Vector2 value) {
      XInput.SetValue(value.x);
      YInput.SetValue(value.y);
    }

    public Vector2 GetValue() {
      return new(XInput.CurrentValue, YInput.CurrentValue);
    }

    static readonly Dictionary<SettingEntryBase, Vector2SettingField> _vector2ConfigCache = new();

    public static void DrawVector2(SettingEntryBase configEntry) {
      Vector2 configValue = (Vector2) configEntry.Get();

      if (!_vector2ConfigCache.TryGetValue(configEntry, out Vector2SettingField cacheEntry)) {
        cacheEntry = new(configValue);
        _vector2ConfigCache[configEntry] = cacheEntry;
      }

      if (GUIFocus.HasChanged() || GUIHelper.IsEnterPressed() || cacheEntry.GetValue() != configValue) {
        cacheEntry.SetValue(configValue);
      }

      cacheEntry.XInput.DrawField();
      cacheEntry.YInput.DrawField();

      Vector2 value = cacheEntry.GetValue();

      if (value != configValue) {
        configEntry.Set(value);
      }
    }
  }
}
