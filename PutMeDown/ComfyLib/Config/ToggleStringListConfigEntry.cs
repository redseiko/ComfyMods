namespace ComfyLib;

using System;
using System.Collections.Generic;

using BepInEx.Configuration;

using UnityEngine;

public sealed class ToggleStringListConfigEntry {
  public readonly ConfigEntry<string> BaseConfigEntry;
  public event EventHandler<string[]> SettingChanged;

  readonly AutoCompleteBox _autoCompleteLabel = default;

  public ToggleStringListConfigEntry(
      ConfigFile config,
      string section,
      string key,
      string defaultValue,
      string description,
      Func<IEnumerable<SearchOption>> autoCompleteFunc = default) {
    BaseConfigEntry = config.BindInOrder(section, key, defaultValue, description, Drawer);
    BaseConfigEntry.SettingChanged += OnBaseSettingChanged;

    if (autoCompleteFunc != default) {
      _autoCompleteLabel = new(autoCompleteFunc);
    }
  }

  void OnBaseSettingChanged(object sender, EventArgs eventArgs) {
    SettingChanged?.Invoke(this, ToggledStringValues());
  }

  readonly List<string> _valuesCache = [];
  string _valueText = string.Empty;

  static bool IsToggledOn(ReadOnlySpan<char> value) {
    return value.Length == 1 && value[0] == '1';
  }

  public string[] ToggledStringValues() {
    _valuesCache.Clear();
    ReadOnlySpan<char> span = BaseConfigEntry.Value.AsSpan();

    while (!span.IsEmpty) {
      int commaIndex = span.IndexOf(',');
      ReadOnlySpan<char> entry = commaIndex >= 0 ? span[..commaIndex] : span;
      span = commaIndex >= 0 ? span[(commaIndex + 1)..] : [];

      if (entry.IsEmpty) {
        continue;
      }

      int equalsIndex = entry.IndexOf('=');

      if (equalsIndex > 0 && IsToggledOn(entry[(equalsIndex + 1)..])) {
        _valuesCache.Add(entry[..equalsIndex].ToString());
      }
    }

    return _valuesCache.ToArray();
  }

  public void Drawer(ConfigEntryBase configEntry) {
    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

    _valuesCache.Clear();
    ReadOnlySpan<char> span = BaseConfigEntry.Value.AsSpan();

    while (!span.IsEmpty) {
      int commaIndex = span.IndexOf(',');
      ReadOnlySpan<char> entry = commaIndex >= 0 ? span[..commaIndex] : span;
      span = commaIndex >= 0 ? span[(commaIndex + 1)..] : [];

      if (!entry.IsEmpty) {
        _valuesCache.Add(entry.ToString());
      }
    }

    GUILayout.BeginHorizontal();
    bool toggleOnClicked = GUILayout.Button("Toggle On", GUILayout.ExpandWidth(true));
    bool toggleOffClicked = GUILayout.Button("Toggle Off", GUILayout.ExpandWidth(true));
    GUILayout.EndHorizontal();

    bool hasChanged = false;
    int removeIndex = -1;

    for (int i = 0, count = _valuesCache.Count; i < count; i++) {
      ReadOnlySpan<char> entry = _valuesCache[i].AsSpan();
      int equalsIndex = entry.IndexOf('=');
      bool isToggled = equalsIndex > 0 && IsToggledOn(entry[(equalsIndex + 1)..]);
      string label = equalsIndex > 0 ? entry[..equalsIndex].ToString() : _valuesCache[i];

      GUILayout.BeginHorizontal();

      bool result = GUILayout.Toggle(isToggled, label, GUILayout.ExpandWidth(true));

      if (GUILayout.Button("\u2212", GUILayout.MinWidth(40f), GUILayout.ExpandWidth(false))) {
        removeIndex = i;
      }

      GUILayout.EndHorizontal();

      if (toggleOnClicked) {
        result = true;
      } else if (toggleOffClicked) {
        result = false;
      }

      if (result != isToggled) {
        hasChanged = true;
        _valuesCache[i] = label + (result ? "=1" : "=0");
      }
    }

    GUILayout.BeginHorizontal();

    _valueText = GUILayout.TextField(_valueText, GUILayout.ExpandWidth(true));
    GUILayout.Space(3f);

    if (GUILayout.Button("\u002B", GUILayout.MinWidth(40f), GUILayout.ExpandWidth(false))
        && !string.IsNullOrWhiteSpace(_valueText)
        && _valueText.IndexOf('=') < 0) {
      _valuesCache.Add(_valueText + "=1");
      _valueText = string.Empty;
      hasChanged = true;
    }

    GUILayout.EndHorizontal();

    if (_autoCompleteLabel != null) {
      string result = _autoCompleteLabel.DrawBox(_valueText);

      if (!string.IsNullOrEmpty(result)) {
        _valuesCache.Add(result + "=1");
        hasChanged = true;
      }
    }

    GUILayout.EndVertical();

    if (removeIndex >= 0) {
      _valuesCache.RemoveAt(removeIndex);
      hasChanged = true;
    }

    if (hasChanged) {
      BaseConfigEntry.Value = string.Join(",", _valuesCache);
    }
  }

  public sealed class SearchOption {
    public string OptionValue = string.Empty;
    public string DisplayValue = string.Empty;

    public SearchOption(string optionValue) : this(optionValue, optionValue) { }

    public SearchOption(string optionValue, string displayValue) {
      OptionValue = optionValue;
      DisplayValue = displayValue;
    }
  }

  public sealed class AutoCompleteBox {
    readonly Func<IEnumerable<SearchOption>> _searchOptionsFunc;
    List<SearchOption> _options;

    readonly List<SearchOption> _currentOptions;
    string _value;
    Vector2 _scrollPosition;

    public AutoCompleteBox(Func<IEnumerable<SearchOption>> searchOptionsFunc) {
      _searchOptionsFunc = searchOptionsFunc;
      _options = null;
      _currentOptions = [];
      _value = string.Empty;
      _scrollPosition = Vector2.zero;
    }

    public string DrawBox(string value) {
      if (_value != value) {
        _value = value;
        FilterOptions(_value);
        _scrollPosition = Vector2.zero;
      }

      if (string.IsNullOrEmpty(_value)) {
        return string.Empty;
      }

      return DrawCurrentOptions();
    }

    void FilterOptions(string query) {
      _currentOptions.Clear();

      if (string.IsNullOrEmpty(query)) {
        return;
      }

      _options ??= [.. _searchOptionsFunc()];

      foreach (SearchOption option in _options) {
        if (option.DisplayValue.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
            || option.OptionValue.StartsWith(query, StringComparison.OrdinalIgnoreCase)) {
          _currentOptions.Add(option);
        }
      }
    }

    string DrawCurrentOptions() {
      _scrollPosition =
          GUILayout.BeginScrollView(
              _scrollPosition, GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(120f));

      string result = string.Empty;

      foreach (SearchOption option in _currentOptions) {
        if (GUILayout.Button(option.DisplayValue, GUILayout.MinWidth(40f))) {
          result = option.OptionValue;
        }
      }

      GUILayout.EndScrollView();

      return result;
    }
  }
}
