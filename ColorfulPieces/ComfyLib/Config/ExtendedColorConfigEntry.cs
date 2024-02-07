namespace ComfyLib;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using BepInEx.Configuration;

using UnityEngine;

public sealed class ExtendedColorConfigEntry {
  static readonly Texture2D _colorTexture = GUIBuilder.CreateColorTexture(10, 10, Color.white);

  public ConfigEntry<Color> ConfigEntry { get; }
  public Color Value { get; private set; }

  public ColorFloatTextField RedInput { get; } = new("R");
  public ColorFloatTextField GreenInput { get; } = new("G");
  public ColorFloatTextField BlueInput { get; } = new("B");
  public ColorFloatTextField AlphaInput { get; } = new("A");

  readonly HexColorTextField _hexInput = new();
  readonly ColorPalette _colorPalette;

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

  public ExtendedColorConfigEntry(
      ConfigFile config,
      string section,
      string key,
      Color defaultValue,
      string description,
      string colorPaletteKey) :
          this(config, section, key, defaultValue, description) {
    ConfigEntry<string> paletteConfigEntry =
        config.BindInOrder(
            section,
            colorPaletteKey,
            $"{ColorUtility.ToHtmlStringRGBA(defaultValue)},FF0000FF,00FF00FF,0000FFFF",
            $"Color palette for: [{section}] {key}",
            browsable: false);

    _colorPalette = new(this, paletteConfigEntry);
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

    if (_colorPalette != null) {
      GUILayout.Space(5f);
      _colorPalette.DrawColorPalette();
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

public sealed class ColorPalette {
  static readonly char[] _partSeparator = { ',' };
  static readonly string _partJoiner = ",";
  static readonly Texture2D _colorTexture = GUIBuilder.CreateColorTexture(10, 10, Color.white);

  readonly ExtendedColorConfigEntry _colorConfigEntry;
  readonly ConfigEntry<string> _paletteConfigEntry;
  readonly List<Color> _paletteColors;

  public ColorPalette(ExtendedColorConfigEntry colorConfigEntry, ConfigEntry<string> paletteConfigEntry) {
    _colorConfigEntry = colorConfigEntry;
    _paletteConfigEntry = paletteConfigEntry;
    _paletteColors = new();

    LoadPalette();
  }

  void LoadPalette() {
    _paletteColors.Clear();

    foreach (
        string part in
            _paletteConfigEntry.Value.Split(_partSeparator, System.StringSplitOptions.RemoveEmptyEntries)) {
      if (ColorUtility.TryParseHtmlString($"#{part}", out Color color)) {
        _paletteColors.Add(color);
      }
    }
  }

  void SavePalette() {
    _paletteConfigEntry.BoxedValue =
        string.Join(_partJoiner, _paletteColors.Select(color => ColorUtility.ToHtmlStringRGBA(color)));
  }

  bool PaletteColorButtons(out int colorIndex) {
    Texture2D original = GUI.skin.button.normal.background;
    GUI.skin.button.normal.background = _colorTexture;
    colorIndex = -1;

    GUILayout.BeginVertical();
    GUILayout.BeginHorizontal();

    for (int i = 0; i < _paletteColors.Count; i++) {
      GUIHelper.BeginColor(_paletteColors[i]);

      if (GUILayout.Button(string.Empty, GUILayout.Width(20f), GUILayout.ExpandWidth(false))) {
        colorIndex = i;
      }

      GUIHelper.EndColor();

      if ((i + 1) < _paletteColors.Count && ((i + 1) % 8) == 0) {
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
      }
    }

    GUILayout.EndHorizontal();
    GUILayout.EndVertical();

    GUI.skin.button.normal.background = original;
    return colorIndex >= 0;
  }

  bool AddColorButton() {
    return GUILayout.Button("\u002B", GUILayout.MinWidth(25f), GUILayout.ExpandWidth(false));
  }

  bool RemoveColorButton() {
    return GUILayout.Button("\u2212", GUILayout.MinWidth(25f), GUILayout.ExpandWidth(false));
  }

  bool ResetColorsButton() {
    return GUILayout.Button("\u2747", GUILayout.MinWidth(25f), GUILayout.ExpandWidth(false));
  }

  public void DrawColorPalette() {
    GUILayout.BeginHorizontal();

    if (AddColorButton()) {
      _paletteColors.Add(_colorConfigEntry.Value);
      SavePalette();
    }

    GUILayout.Space(2f);

    if (PaletteColorButtons(out int colorIndex)) {
      if (Event.current.button == 0) {
        _colorConfigEntry.SetValue(_paletteColors[colorIndex]);
      } else if (Event.current.button == 1 && colorIndex >= 0 && colorIndex < _paletteColors.Count) {
        _paletteColors.RemoveAt(colorIndex);
        SavePalette();
      }
    }

    GUILayout.FlexibleSpace();

    if (_paletteColors.Count > 0) {
      if (RemoveColorButton()) {
        _paletteColors.RemoveAt(_paletteColors.Count - 1);
        SavePalette();
      }
    } else {
      if (ResetColorsButton()) {
        _paletteConfigEntry.BoxedValue = _paletteConfigEntry.DefaultValue;
        LoadPalette();
      }
    }

    GUILayout.EndHorizontal();
  }
}

public sealed class ColorFloatTextField {
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

public sealed class HexColorTextField {
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
