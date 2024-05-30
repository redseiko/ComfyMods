namespace ComfyLib;

using System;
using System.Collections.Generic;

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
  public static readonly char[] ToggleSeparator = ['='];

  readonly List<string> _valuesCache = [];
  string _valueText = string.Empty;

  public void Drawer(ConfigEntryBase configEntry) {
    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

    _valuesCache.Clear();
    _valuesCache.AddRange(
        configEntry.BoxedValue.ToString().Split(ValueSeparator, StringSplitOptions.RemoveEmptyEntries));

    bool hasChanged = false;
    int removeIndex = -1;

    for (int i = 0, count = _valuesCache.Count; i < count; i++) {
      string[] parts = _valuesCache[i].Split(ToggleSeparator, 3, StringSplitOptions.RemoveEmptyEntries);

      int sliderValue = parts.Length >= 3 && int.TryParse(parts[1], out sliderValue) ? sliderValue : 0;
      bool isToggled = parts.Length >= 3 && parts[2] == "1";

      GUILayout.BeginHorizontal();

      bool toggleResult = GUILayout.Toggle(isToggled, parts[0], GUILayout.ExpandWidth(true));
      int sliderResult = Mathf.RoundToInt(GUILayout.HorizontalSlider(sliderValue, 0, 100, GUILayout.Width(120f)));

      if (GUILayout.Button("\u2212", GUILayout.MinWidth(40f), GUILayout.ExpandWidth(false))) {
        removeIndex = i;
      }

      GUILayout.EndHorizontal();

      if (toggleResult != isToggled || sliderResult != sliderValue) {
        hasChanged = true;
        _valuesCache[i] = $"{parts[0]}={sliderResult:###}={(toggleResult ? "1" : "0")}";
      }
    }

    GUILayout.BeginHorizontal();
    _valueText = GUILayout.TextField(_valueText, GUILayout.ExpandWidth(true));

    GUILayout.Space(3f);

    if (GUILayout.Button("\u002B", GUILayout.MinWidth(40f), GUILayout.ExpandWidth(false))
        && !string.IsNullOrWhiteSpace(_valueText)
        && _valueText.IndexOf('=') < 0) {
      _valuesCache.Add(_valueText + "=100=1");
      _valueText = string.Empty;
      hasChanged = true;
    }

    GUILayout.EndHorizontal();

    if (_autoCompleteBox != default) {
      string result = _autoCompleteBox.DrawBox(_valueText);

      if (!string.IsNullOrEmpty(result)) {
        _valuesCache.Add(result + "=100=1");
        hasChanged = true;
      }
    }

    GUILayout.EndVertical();

    if (removeIndex >= 0) {
      _valuesCache.RemoveAt(removeIndex);
      hasChanged = true;
    }

    if (hasChanged) {
      configEntry.BoxedValue = string.Join(",", _valuesCache);
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
      if (OptionValue.StartsWith(value, StringComparison.OrdinalIgnoreCase)) {
        return true;
      }

      if (DisplayValue.Length > 0 && DisplayValue.IndexOf(value, StringComparison.OrdinalIgnoreCase) > 0) {
        return true;
      }

      return false;
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
        if (!option.IsMatching(_value)) {
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
