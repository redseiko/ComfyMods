namespace Configula;

using System.Collections.Generic;

using ConfigurationManager;

using UnityEngine;

public sealed class QuaternionSettingField {
  public readonly FloatInputField XInput = new("X");
  public readonly FloatInputField YInput = new("Y");
  public readonly FloatInputField ZInput = new("Z");
  public readonly FloatInputField WInput = new("W");

  public void SetValue(Quaternion value) {
    XInput.SetValue(value.x);
    YInput.SetValue(value.y);
    ZInput.SetValue(value.z);
    WInput.SetValue(value.w);
  }

  public Quaternion GetValue() {
    return new(XInput.CurrentValue, YInput.CurrentValue, ZInput.CurrentValue, WInput.CurrentValue);
  }

  public void DrawField() {
    XInput.DrawField();
    YInput.DrawField();
    ZInput.DrawField();
    WInput.DrawField();
  }

  static readonly Dictionary<SettingEntryBase, QuaternionSettingField> _quaternionSettingFieldCache = new();

  public static void DrawQuaternion(SettingEntryBase configEntry) {
    Quaternion configValue = (Quaternion) configEntry.Get();

    if (!_quaternionSettingFieldCache.TryGetValue(configEntry, out QuaternionSettingField quaternionField)) {
      quaternionField = new();
      quaternionField.SetValue(configValue);

      _quaternionSettingFieldCache[configEntry] = quaternionField;
    } else if (GUIFocus.HasChanged() || GUIHelper.IsEnterPressed() || quaternionField.GetValue() != configValue) {
      quaternionField.SetValue(configValue);
    }

    quaternionField.DrawField();
    Quaternion value = quaternionField.GetValue();

    if (value != configValue) {
      configEntry.Set(value);
    }
  }
}
