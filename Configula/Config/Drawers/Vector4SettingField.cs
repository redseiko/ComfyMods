namespace Configula;

using System.Collections.Generic;

using ConfigurationManager;

using UnityEngine;

public sealed class Vector4SettingField {
  public readonly FloatInputField XInput = new("X");
  public readonly FloatInputField YInput = new("Y");
  public readonly FloatInputField ZInput = new("Z");
  public readonly FloatInputField WInput = new("W");

  public void SetValue(Vector4 value) {
    XInput.SetValue(value.x);
    YInput.SetValue(value.y);
    ZInput.SetValue(value.z);
    WInput.SetValue(value.w);
  }

  public Vector4 GetValue() {
    return new(XInput.CurrentValue, YInput.CurrentValue, ZInput.CurrentValue, WInput.CurrentValue);
  }

  public void DrawField() {
    XInput.DrawField();
    YInput.DrawField();
    ZInput.DrawField();
    WInput.DrawField();
  }

  static readonly Dictionary<SettingEntryBase, Vector4SettingField> _vector4SettingFieldCache = new();

  public static void DrawVector4(SettingEntryBase configEntry) {
    Vector4 configValue = (Vector4) configEntry.Get();

    if (!_vector4SettingFieldCache.TryGetValue(configEntry, out Vector4SettingField vector4Field)) {
      vector4Field = new();
      vector4Field.SetValue(configValue);

      _vector4SettingFieldCache[configEntry] = vector4Field;
    } else if (GUIFocus.HasChanged() || GUIHelper.IsEnterPressed() || vector4Field.GetValue() != configValue) {
      vector4Field.SetValue(configValue);
    }

    vector4Field.DrawField();
    Vector4 value = vector4Field.GetValue();

    if (value != configValue) {
      configEntry.Set(value);
    }
  }
}
