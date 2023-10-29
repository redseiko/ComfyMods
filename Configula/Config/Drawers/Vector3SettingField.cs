using System.Collections.Generic;

using ConfigurationManager;

using UnityEngine;

namespace Configula {
  public class Vector3SettingField {
    public readonly FloatInputField XInput = new("X");
    public readonly FloatInputField YInput = new("Y");
    public readonly FloatInputField ZInput = new("Z");

    public void SetValue(Vector3 value) {
      XInput.SetValue(value.x);
      YInput.SetValue(value.y);
      ZInput.SetValue(value.z);
    }

    public Vector3 GetValue() {
      return new(XInput.CurrentValue, YInput.CurrentValue, ZInput.CurrentValue);
    }

    public void DrawField() {
      XInput.DrawField();
      YInput.DrawField();
      ZInput.DrawField();
    }

    static readonly Dictionary<SettingEntryBase, Vector3SettingField> _vector3SettingFieldCache = new();

    public static void DrawVector3(SettingEntryBase configEntry) {
      Vector3 configValue = (Vector3) configEntry.Get();

      if (!_vector3SettingFieldCache.TryGetValue(configEntry, out Vector3SettingField vector3Field)) {
        vector3Field = new();
        vector3Field.SetValue(configValue);

        _vector3SettingFieldCache[configEntry] = vector3Field;
      } else if (GUIFocus.HasChanged() || GUIHelper.IsEnterPressed() || vector3Field.GetValue() != configValue) {
        vector3Field.SetValue(configValue);
      }

      vector3Field.DrawField();

      Vector3 value = vector3Field.GetValue();

      if (value != configValue) {
        configEntry.Set(value);
      }
    }
  }
}
