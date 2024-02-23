namespace Configula;

using System.Globalization;

using UnityEngine;

public sealed class ColorFloatInputField {
  public string FieldLabel { get; set; }
  public float CurrentValue { get; private set; }

  public float MinValue { get; private set; }
  public float MaxValue { get; private set; }

  public void SetValue(float value) {
    CurrentValue = Mathf.Clamp(value, MinValue, MaxValue);

    _fieldText = value.ToString("F3", CultureInfo.InvariantCulture);
    _fieldColor = GUI.color;
  }

  public void SetValueRange(float minValue, float maxValue) {
    MinValue = Mathf.Min(minValue, minValue);
    MaxValue = Mathf.Max(maxValue, maxValue);
  }

  string _fieldText;
  Color _fieldColor;

  public ColorFloatInputField(string label) {
    FieldLabel = label;
    SetValueRange(0f, 1f);
  }

  public void DrawField() {
    GUILayout.BeginVertical();

    GUILayout.BeginHorizontal();
    GUILayout.Label(FieldLabel, GUILayout.ExpandWidth(true));

    GUIHelper.BeginColor(_fieldColor);

    string textValue =
        GUILayout.TextField(
            _fieldText, GUILayout.MinWidth(45f), GUILayout.MaxWidth(55f), GUILayout.ExpandWidth(true));

    GUIHelper.EndColor();

    GUILayout.EndHorizontal();
    GUILayout.Space(2f);

    float sliderValue = GUILayout.HorizontalSlider(CurrentValue, MinValue, MaxValue, GUILayout.ExpandWidth(true));

    GUILayout.EndVertical();

    if (sliderValue != CurrentValue) {
      SetValue(sliderValue);
      return;
    }

    if (textValue == _fieldText) {
      return;
    }

    if (float.TryParse(textValue, NumberStyles.Any, CultureInfo.InvariantCulture, out float result)
        && result >= MinValue
        && result <= MaxValue) {
      CurrentValue = result;
      _fieldColor = GUI.color;
    } else {
      _fieldColor = Color.red;
    }

    _fieldText = textValue;
  }
}
