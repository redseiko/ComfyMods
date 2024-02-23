namespace Configula;

using System.Collections.Generic;

using ConfigurationManager;

using UnityEngine;

public sealed class Vector2SettingField {
  public readonly FloatInputField XInput = new("X");
  public readonly FloatInputField YInput = new("Y");

  public void SetValue(Vector2 value) {
    XInput.SetValue(value.x);
    YInput.SetValue(value.y);
  }

  public Vector2 GetValue() {
    return new(XInput.CurrentValue, YInput.CurrentValue);
  }

  public void DrawField() {
    XInput.DrawField();
    YInput.DrawField();
  }

  static readonly Dictionary<SettingEntryBase, Vector2SettingField> _vector2SettingFieldCache = new();

  public static void DrawVector2(SettingEntryBase configEntry) {
    Vector2 configValue = (Vector2) configEntry.Get();

    if (!_vector2SettingFieldCache.TryGetValue(configEntry, out Vector2SettingField vector2Field)) {
      vector2Field = new();
      vector2Field.SetValue(configValue);

      _vector2SettingFieldCache[configEntry] = vector2Field;
    } else if (GUIFocus.HasChanged() || GUIHelper.IsEnterPressed() || vector2Field.GetValue() != configValue) {
      vector2Field.SetValue(configValue);
    }

    vector2Field.DrawField();
    Vector2 value = vector2Field.GetValue();

    if (value != configValue) {
      configEntry.Set(value);
    }
  }
}
