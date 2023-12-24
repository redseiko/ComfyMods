using System.Globalization;

using BepInEx.Configuration;

using UnityEngine;

namespace ComfyLib {
  public class ExtendedColorConfigEntry {
    static readonly Texture2D _colorTexture = GUIBuilder.CreateColorTexture(10, 10, Color.white);

    public ConfigEntry<Color> ConfigEntry { get; }
    public Color Value { get; private set; }

    public ColorFloatTextField RedInput { get; } = new("R");
    public ColorFloatTextField GreenInput { get; } = new("G");
    public ColorFloatTextField BlueInput { get; } = new("B");
    public ColorFloatTextField AlphaInput { get; } = new("A");

    readonly HexColorTextField _hexInput = new();

    bool _showSliders = false;

    public ExtendedColorConfigEntry(
        ConfigFile config,
        string section,
        string key,
        Color defaultValue,
        string description) {
      ConfigEntry = config.BindInOrder(section, key, defaultValue, description, Drawer);
      SetValue(ConfigEntry.Value);
    }

    public void SetValue(Color value) {
      ConfigEntry.Value = value;
      Value = value;

      RedInput.SetValue(value.r);
      GreenInput.SetValue(value.g);
      BlueInput.SetValue(value.b);
      AlphaInput.SetValue(value.a);
      _hexInput.SetValue(value);
    }

    public void Drawer(ConfigEntryBase configEntry) {
      Color configColor = (Color) configEntry.BoxedValue;

      if (GUIFocus.HasChanged() || GUIHelper.IsEnterPressed() || Value != configColor) {
        SetValue(configColor);
      }

      GUILayout.BeginVertical(GUI.skin.box);
      GUILayout.BeginHorizontal();
      _hexInput.DrawField();

      GUILayout.Space(3f);
      GUIHelper.BeginColor(configColor);
      GUILayout.Label(string.Empty, GUILayout.ExpandWidth(true));

      if (Event.current.type == EventType.Repaint) {
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), _colorTexture);
      }

      GUIHelper.EndColor();
      GUILayout.Space(3f);

      if (GUILayout.Button(
            _showSliders ? "\u2228" : "\u2261", GUILayout.MinWidth(40f), GUILayout.ExpandWidth(false))) {
        _showSliders = !_showSliders;
      }

      GUILayout.EndHorizontal();

      if (_showSliders) {
        GUILayout.Space(4f);
        GUILayout.BeginHorizontal();

        RedInput.DrawField();
        GUILayout.Space(3f);
        GreenInput.DrawField();
        GUILayout.Space(3f);
        BlueInput.DrawField();
        GUILayout.Space(3f);
        AlphaInput.DrawField();

        GUILayout.EndHorizontal();
      }

      GUILayout.EndVertical();

      Color sliderColor =
          new(RedInput.CurrentValue, GreenInput.CurrentValue, BlueInput.CurrentValue, AlphaInput.CurrentValue);

      if (sliderColor != configColor) {
        configEntry.BoxedValue = sliderColor;
        SetValue(sliderColor);
      } else if (_hexInput.CurrentValue != configColor) {
        configEntry.BoxedValue = _hexInput.CurrentValue;
        SetValue(_hexInput.CurrentValue);
      }
    }
  }

  public class ColorFloatTextField {
    public string Label { get; set; }
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

    public ColorFloatTextField(string label) {
      Label = label;
      SetValueRange(0f, 1f);
    }

    public void DrawField() {
      GUILayout.BeginVertical();

      GUILayout.BeginHorizontal();
      GUILayout.Label(Label, GUILayout.ExpandWidth(true));

      GUIHelper.BeginColor(_fieldColor);

      string textValue =
          GUILayout.TextField(_fieldText, GUILayout.MinWidth(45f), GUILayout.MaxWidth(55f), GUILayout.ExpandWidth(true));

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

  public class HexColorTextField {
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
