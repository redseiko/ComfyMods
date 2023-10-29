using UnityEngine;

namespace Configula {
  public class HexColorInputField {
    public Color CurrentValue { get; private set; }
    public string CurrentText { get; private set; }

    Color _textColor = GUI.color;

    public void SetValue(Color value) {
      CurrentValue = value;
      CurrentText = $"#{(value.a == 1f ? ColorUtility.ToHtmlStringRGB(value) : ColorUtility.ToHtmlStringRGBA(value))}";

      _textColor = GUI.color;
    }

    public void DrawField() {
      GUIHelper.BeginColor(_textColor);
      string textValue = GUILayout.TextField(CurrentText, GUILayout.Width(90f), GUILayout.ExpandWidth(false));
      GUIHelper.EndColor();

      if (textValue == CurrentText) {
        return;
      }

      CurrentText = textValue;

      if (ColorUtility.TryParseHtmlString(textValue, out Color color)) {
        CurrentValue = color;
      } else {
        _textColor = Color.red;
      }
    }
  }
}
