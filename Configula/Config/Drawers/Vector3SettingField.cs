using System.Collections.Generic;

using ConfigurationManager;

using UnityEngine;

namespace Configula {
  public class Vector3SettingField {
    public FloatInputField XInput;
    public FloatInputField YInput;
    public FloatInputField ZInput;

    public Vector3SettingField(Vector3 value) {
      XInput = new("X");
      YInput = new("Y");
      ZInput = new("Z");
    }

    public void SetValue(Vector3 value) {
      XInput.SetValue(value.x);
      YInput.SetValue(value.y);
      ZInput.SetValue(value.z);
    }

    public Vector3 GetValue() {
      return new(XInput.CurrentValue, YInput.CurrentValue, ZInput.CurrentValue);
    }

    static readonly Dictionary<SettingEntryBase, Vector3SettingField> _vector3ConfigCache = new();

    public static void DrawVector3(SettingEntryBase configEntry) {
      Vector3 configValue = (Vector3) configEntry.Get();

      if (!_vector3ConfigCache.TryGetValue(configEntry, out Vector3SettingField cacheEntry)) {
        cacheEntry = new(configValue);
        _vector3ConfigCache[configEntry] = cacheEntry;
      }

      if (GUIFocus.HasChanged() || GUIHelper.IsEnterPressed() || cacheEntry.GetValue() != configValue) {
        cacheEntry.SetValue(configValue);
      }

      cacheEntry.XInput.DrawField();
      cacheEntry.YInput.DrawField();
      cacheEntry.ZInput.DrawField();

      Vector3 value = cacheEntry.GetValue();

      if (value != configValue) {
        configEntry.Set(value);
      }
    }
  }
}
