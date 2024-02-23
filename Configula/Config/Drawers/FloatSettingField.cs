namespace Configula;

using System.Collections.Generic;
using System.Globalization;

using ConfigurationManager;

using UnityEngine;

public sealed class FloatSettingField {
  public readonly FloatInputField ValueInput = new(string.Empty);

  public void SetValue(float value) {
    ValueInput.SetValue(value);
  }

  public float GetValue() {
    return ValueInput.CurrentValue;
  }

  public void DrawField() {
    ValueInput.DrawField();
  }

  static readonly Dictionary<SettingEntryBase, FloatSettingField> _floatSettingFieldCache = new();

  public static void DrawFloat(SettingEntryBase configEntry) {
    float configValue = (float) configEntry.Get();

    if (!_floatSettingFieldCache.TryGetValue(configEntry, out FloatSettingField floatField)) {
      floatField = new();
      floatField.SetValue(configValue);

      _floatSettingFieldCache[configEntry] = floatField;
    } else if (GUIFocus.HasChanged() || GUIHelper.IsEnterPressed() || floatField.GetValue() != configValue) {
      floatField.SetValue(configValue);
    }

    floatField.DrawField();
    float value = floatField.GetValue();

    if (value != configValue) {
      configEntry.Set(value);
    }
  }
}
