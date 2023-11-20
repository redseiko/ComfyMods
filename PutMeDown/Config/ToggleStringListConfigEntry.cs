using System;
using System.Collections.Generic;
using System.Linq;

using BepInEx.Configuration;

using UnityEngine;

namespace ComfyLib {
  public class ToggleStringListConfigEntry {
    public readonly ConfigEntry<string> BaseConfigEntry;
    public event EventHandler<string[]> SettingChanged;

    readonly AutoCompleteLabel _autoCompleteLabel = default;

    public ToggleStringListConfigEntry(
        ConfigFile config,
        string section,
        string key,
        string defaultValue,
        string description,
        Func<IEnumerable<string>> autoCompleteFunc = default) {
      BaseConfigEntry = config.BindInOrder(section, key, defaultValue, description, Drawer);
      BaseConfigEntry.SettingChanged += (sender, _) => SettingChanged?.Invoke(sender, ToggledStringValues());

      if (autoCompleteFunc != default) {
        _autoCompleteLabel = new(autoCompleteFunc);
      }
    }

    static readonly char[] _valueSeparator = { ',' };
    static readonly char[] _toggleSeperator = { '=' };

    readonly List<string> _valuesCache = new();
    string _valueText = string.Empty;

    public string[] ToggledStringValues() {
      _valuesCache.Clear();
      string[] values = BaseConfigEntry.Value.Split(_valueSeparator, StringSplitOptions.RemoveEmptyEntries);

      for (int i = 0, count = values.Length; i < count; i++) {
        string[] parts = values[i].Split(_toggleSeperator, 2, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length >= 2 && parts[1] == "1") {
          _valuesCache.Add(parts[0]);
        }
      }

      return _valuesCache.ToArray();
    }

    public void Drawer(ConfigEntryBase configEntry) {
      GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

      _valuesCache.Clear();

      _valuesCache.AddRange(
          configEntry.BoxedValue.ToString().Split(_valueSeparator, StringSplitOptions.RemoveEmptyEntries));

      GUILayout.BeginHorizontal();
      bool toggleOnClicked = GUILayout.Button("Toggle On", GUILayout.ExpandWidth(true));
      bool toggleOffClicked = GUILayout.Button("Toggle Off", GUILayout.ExpandWidth(true));
      GUILayout.EndHorizontal();

      bool hasChanged = false;
      int removeIndex = -1;

      for (int i = 0, count = _valuesCache.Count; i < count; i++) {
        string[] parts = _valuesCache[i].Split(_toggleSeperator, 2, StringSplitOptions.RemoveEmptyEntries);
        bool isToggled = parts.Length >= 2 && parts[1] == "1";

        GUILayout.BeginHorizontal();

        bool result = GUILayout.Toggle(isToggled, parts[0], GUILayout.ExpandWidth(true));

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
          _valuesCache[i] = parts[0] + (result ? "=1" : "=0");
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

      _autoCompleteLabel?.DrawLabel(_valueText);

      GUILayout.EndVertical();

      if (removeIndex >= 0) {
        _valuesCache.RemoveAt(removeIndex);
        hasChanged = true;
      }

      if (hasChanged) {
        configEntry.BoxedValue = string.Join(",", _valuesCache);
      }
    }

    public class AutoCompleteLabel {
      static readonly Lazy<GUIStyle> _labelStyle =
          new(() => new(GUI.skin.label) {
            padding = new(1, 1, 1, 1),
            margin = new(1, 1, 1, 1)
          });

      readonly Func<IEnumerable<string>> _optionsFunc;
      List<string> _options;
      string _value;
      string _labelText;
      Vector2 _scrollPosition;

      public AutoCompleteLabel(Func<IEnumerable<string>> optionsFunc) {
        _optionsFunc = optionsFunc;
        _options = null;
        _value = string.Empty;
        _labelText = string.Empty;
        _scrollPosition = Vector2.zero;
      }

      public void DrawLabel(string value) {
        if (_value != value) {
          _value = value;
          _labelText = GetLabelText(value);
          _scrollPosition = Vector2.zero;
        }

        if (!string.IsNullOrEmpty(_value)) {
          _scrollPosition =
              GUILayout.BeginScrollView(
                  _scrollPosition, GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(50f));

          GUILayout.Label(_labelText, _labelStyle.Value);
          GUILayout.EndScrollView();
        }
      }

      string GetLabelText(string value) {
        if (string.IsNullOrEmpty(value)) {
          return string.Empty;
        }

        _options ??= new(_optionsFunc());

        if (_options.Count <= 0) {
          return string.Empty;
        }

        return string.Join(
            ", ",
            _options
                .Where(option => option.StartsWith(value, StringComparison.InvariantCultureIgnoreCase))
                .Take(10));
      }
    }
  }
}
