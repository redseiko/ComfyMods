namespace ComfyLib;

using System;
using System.Collections.Generic;
using System.Globalization;

using BepInEx.Configuration;

using UnityEngine;

public sealed class ToggleSliderListConfigEntry {
  public ConfigEntry<string> ConfigEntry { get; }

  readonly AutoCompleteBox _autoCompleteBox = default;

  public ToggleSliderListConfigEntry(
      ConfigFile config,
      string section,
      string key,
      string defaultValue,
      string description,
      Func<IEnumerable<SearchOption>> autoCompleteFunc = default) {
    ConfigEntry = config.BindInOrder(section, key, defaultValue, description, Drawer);

    if (autoCompleteFunc != default) {
      _autoCompleteBox = new(autoCompleteFunc);
    }
  }

  public static readonly char[] ValueSeparator = [','];
  public static readonly char[] ToggleSeparator = [';'];

  public IEnumerable<(string, float)> GetToggledValues() {
    string[] values = ConfigEntry.Value.Split(ValueSeparator, StringSplitOptions.RemoveEmptyEntries);

    foreach (string value in values) {
      string[] parts = value.Split(ToggleSeparator, 3, StringSplitOptions.RemoveEmptyEntries);

      if (parts.Length >= 3
          && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float sliderValue)
          && parts[2] == "1") {
        yield return (parts[0], sliderValue);
      }
    }
  }

  readonly List<string> _valuesCache = [];
  string _valueText = string.Empty;

  static readonly Lazy<GUIStyle> _sliderStyle = new(
      () => new GUIStyle(GUI.skin.horizontalSlider) {
        margin = new(4, 4, 9, 9)
      });

  static readonly Lazy<GUIStyle> _toggleStyle = new(
      () => new GUIStyle(GUI.skin.toggle) {
        padding = new(25, 0, 3, 3)
      });

  bool _hasChanged = false;
  int _removeIndex = -1;
  bool _toggleResult = false;
  float _sliderResult = 0f;

  void DrawValue(int index, string label, float sliderValue, bool isToggled) {
    GUILayout.BeginVertical(GUI.skin.box);
    GUILayout.BeginHorizontal();

    _toggleResult = GUILayout.Toggle(isToggled, label, _toggleStyle.Value, GUILayout.ExpandWidth(true));

    if (GUILayout.Button("\u2212", GUILayout.MinWidth(40f), GUILayout.ExpandWidth(false))) {
      _removeIndex = index;
    }

    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();

    _sliderResult =
        GUILayout.HorizontalSlider(
            sliderValue, 0f, 1f, _sliderStyle.Value, GUI.skin.horizontalSliderThumb, GUILayout.ExpandWidth(true));

    GUILayout.Space(3f);
    GUILayout.Label($"{sliderValue:P0}", GUILayout.Width(40f));

    GUILayout.EndHorizontal();
    GUILayout.EndVertical();
  }

  public void Drawer(ConfigEntryBase configEntry) {
    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

    _valuesCache.Clear();
    _valuesCache.AddRange(
        configEntry.BoxedValue.ToString().Split(ValueSeparator, StringSplitOptions.RemoveEmptyEntries));

    _hasChanged = false;
    _removeIndex = -1;

    for (int i = 0, count = _valuesCache.Count; i < count; i++) {
      string[] parts = _valuesCache[i].Split(ToggleSeparator, 3, StringSplitOptions.RemoveEmptyEntries);

      if (parts.Length < 3
          || !float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float sliderValue)) {
        sliderValue = 0f;
      }

      bool isToggled = parts.Length >= 3 && parts[2] == "1";

      DrawValue(i, parts[0], sliderValue, isToggled);

      if (_toggleResult != isToggled || _sliderResult != sliderValue) {
        _hasChanged = true;
        _valuesCache[i] = $"{parts[0]};{_sliderResult};{(_toggleResult ? "1" : "0")}";
      }
    }

    GUILayout.BeginHorizontal();
    _valueText = GUILayout.TextField(_valueText, GUILayout.ExpandWidth(true));

    GUILayout.Space(3f);

    if (GUILayout.Button("\u002B", GUILayout.MinWidth(40f), GUILayout.ExpandWidth(false))
        && !string.IsNullOrWhiteSpace(_valueText)
        && _valueText.IndexOf(';') < 0) {
      _valuesCache.Add(_valueText + ";1;1");
      _valueText = string.Empty;
      _hasChanged = true;
    }

    GUILayout.EndHorizontal();

    if (_autoCompleteBox != default) {
      string result = _autoCompleteBox.DrawBox(_valueText);

      if (!string.IsNullOrEmpty(result)) {
        _valuesCache.Add(result + ";1;1");
        _hasChanged = true;
      }
    }

    GUILayout.EndVertical();

    if (_removeIndex >= 0) {
      _valuesCache.RemoveAt(_removeIndex);
      _hasChanged = true;
    }

    if (_hasChanged) {
      configEntry.BoxedValue = string.Join(",", _valuesCache);
    }
  }

  public sealed class AutoCompleteBox {
    readonly Func<IEnumerable<SearchOption>> _searchOptionsFunc;

    string _value;
    Vector2 _scrollPosition;

    public AutoCompleteBox(Func<IEnumerable<SearchOption>> searchOptionsFunc) {
      _searchOptionsFunc = searchOptionsFunc;
      SetValue(string.Empty);
    }

    public string DrawBox(string value) {
      if (_value != value) {
        SetValue(value);
      }

      return DrawCurrentOptions();
    }

    void SetValue(string value) {
      _value = value;
      _scrollPosition = Vector2.zero;
    }

    string DrawCurrentOptions() {
      _scrollPosition =
          GUILayout.BeginScrollView(
              _scrollPosition, GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(120f));

      string result = string.Empty;

      foreach (SearchOption option in _searchOptionsFunc()) {
        if (_value.Length > 0 && !option.IsMatching(_value)) {
          continue;
        }

        string displayValue = option.DisplayValue;

        if (displayValue.Length <= 0) {
          displayValue = option.OptionValue;
        }

        if (GUILayout.Button(displayValue, GUILayout.MinWidth(40f))) {
          result = option.OptionValue;
        }
      }

      GUILayout.EndScrollView();

      return result;
    }
  }
}

public sealed class SearchOption {
  public readonly string OptionValue;
  public readonly string DisplayValue;

  public SearchOption(string optionValue) {
    OptionValue = optionValue;
    DisplayValue = string.Empty;
  }

  public SearchOption(string optionValue, string displayValue) {
    OptionValue = optionValue;
    DisplayValue = displayValue;
  }

  public bool IsMatching(string value) {
    if (OptionValue.Length > 0 && OptionValue.IndexOf(value, StringComparison.OrdinalIgnoreCase) > 0) {
      return true;
    }

    if (DisplayValue.Length > 0 && DisplayValue.IndexOf(value, StringComparison.OrdinalIgnoreCase) > 0) {
      return true;
    }

    return false;
  }
}
