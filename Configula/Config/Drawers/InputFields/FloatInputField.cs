namespace Configula;

using System.Globalization;

using UnityEngine;

public sealed class FloatInputField {
  public string FieldLabel;
  public float CurrentValue;
  public string CurrentText;
  public Color CurrentColor;

  public FloatInputField(string label) {
    FieldLabel = label;
    CurrentValue = 0f;
    CurrentText = string.Empty;
    CurrentColor = GUI.color;
  }

  public void SetValue(float value) {
    CurrentValue = value;
    CurrentText = value.ToString(NumberFormatInfo.InvariantInfo);
    CurrentColor = GUI.color;
  }

  public void DrawField() {
    GUILayout.Label(FieldLabel, GUILayout.ExpandWidth(false));

    GUIHelper.BeginColor(CurrentColor);

    string textValue =
        GUILayout.TextField(
            CurrentText, GUIResources.WordWrapTextField.Value, GUILayout.MinWidth(45f), GUILayout.ExpandWidth(true));

    GUIHelper.EndColor();

    if (textValue == CurrentText) {
      return;
    }

    if (ShouldParse(textValue)
        && float.TryParse(textValue, NumberStyles.Any, CultureInfo.InvariantCulture, out float result)) {
      CurrentValue = result;
      CurrentColor = GUI.color;
    } else {
      CurrentColor = Color.red;
    }

    CurrentText = textValue;
  }

  static bool ShouldParse(string text) {
    if (text == null || text.Length <= 0) {
      return false;
    }

    return text[text.Length - 1] switch {
      'e' or 'E' or '+' or '-' or '.' or ',' => false,
      _ => true,
    };
  }
}
