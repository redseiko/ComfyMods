using System;
using System.Collections.Generic;
using System.Linq;

using BepInEx.Configuration;

using UnityEngine;

namespace ComfyLib {
  public sealed class StringListConfigEntry {
    public ConfigEntry<string> ConfigEntry { get; }

    public string[] ValuesSeparator { get; }

    public List<string> Values {
      get => ConfigEntry.Value.Split(ValuesSeparator, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public List<string> CachedValues { get; }

    public event EventHandler<List<string>> ValuesChangedEvent;

    public StringListConfigEntry(
        ConfigFile configFile, string section, string key, string description, string valuesSeparator) {
      ConfigEntry = configFile.BindInOrder(
          section,
          key,
          defaultValue: string.Empty,
          description,
          customDrawer: Drawer,
          hideDefaultButton: true,
          hideSettingName: true);

      ValuesSeparator = new string[] { valuesSeparator };
      CachedValues = new(Values);
    }

    readonly List<string> _valuesCache = new();

    void Drawer(ConfigEntryBase entry) {
      GUILayout.BeginVertical();

      GUILayout.BeginHorizontal(_horizontalStyle.Value);
      GUILayout.Label(
          $"<b>{entry.Definition.Key}</b>\n<i>{entry.Description.Description}</i>",
          _richTextLabelStyle.Value,
          GUILayout.ExpandWidth(true));
      GUILayout.EndHorizontal();

      _valuesCache.Clear();

      if (!string.IsNullOrEmpty(entry.BoxedValue.ToString())) {
        _valuesCache.AddRange(entry.BoxedValue.ToString().Split(ValuesSeparator, StringSplitOptions.None));
      }

      int deleteIndex = -1;
      bool valuesChanged = false;

      if (!string.IsNullOrEmpty(entry.BoxedValue.ToString())) {
        for (int i = 0; i < _valuesCache.Count; i++) {
          GUILayout.BeginHorizontal(_horizontalStyle.Value);
          GUILayout.Space(pixels: 5f);
          GUILayout.Label($"#{i:D2}", _labelStyle.Value, GUILayout.ExpandWidth(false));
          GUILayout.Space(pixels: 5f);

          string name = $"{entry.Definition.Key}.{i}";
          string value = _valuesCache[i];

          if (ShowTextField(name, ref value)) {
            if (_valuesCache[i] != value) {
              _valuesCache[i] = value;
              valuesChanged = true;
            }
          }

          GUILayout.Space(pixels: 10f);

          if (GUILayout.Button("Delete", _buttonStyle.Value, GUILayout.ExpandWidth(false))) {
            deleteIndex = i;
          }

          GUILayout.EndHorizontal();
        }
      }

      GUILayout.BeginHorizontal(_horizontalStyle.Value);
      GUILayout.Space(pixels: 10f);

      if (GUILayout.Button("Add new entry", _buttonStyle.Value, GUILayout.ExpandWidth(false))) {
        _valuesCache.Add("changemeplease");
        valuesChanged = true;
      }

      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      GUILayout.EndVertical();

      if (deleteIndex >= 0 && deleteIndex < _valuesCache.Count) {
        _valuesCache.RemoveAt(deleteIndex);
        valuesChanged = true;
      }

      if (valuesChanged) {
        entry.BoxedValue = string.Join(ValuesSeparator[0], _valuesCache);

        CachedValues.Clear();
        CachedValues.AddRange(Values);

        ValuesChangedEvent?.Invoke(this, Values);
      }
    }

    string _lastFocusedControl;
    string _editingValue;

    bool ShowTextField(string name, ref string value) {
      GUI.SetNextControlName(name);

      if (GUI.GetNameOfFocusedControl() != name) {
        if (_lastFocusedControl == name) {
          value = _editingValue;
          GUILayout.TextField(value, _textFieldStyle.Value, GUILayout.ExpandWidth(true));
          return true;
        }

        GUILayout.TextField(value, _textFieldStyle.Value, GUILayout.ExpandWidth(true));
        return false;
      }

      if (_lastFocusedControl != name) {
        _lastFocusedControl = name;
        _editingValue = value;
      }

      bool applyingValue = false;

      if (Event.current.isKey) {
        switch (Event.current.keyCode) {
          case KeyCode.Tab:
          case KeyCode.Return:
          case KeyCode.KeypadEnter:
          case KeyCode.Escape:
            value = _editingValue;
            applyingValue = true;
            Event.current.Use();
            break;
        }
      }

      _editingValue = GUILayout.TextField(_editingValue, _textFieldStyle.Value, GUILayout.ExpandWidth(true));

      return applyingValue;
    }

    static readonly Lazy<GUIStyle> _richTextLabelStyle =
        new(() =>
          new(GUI.skin.label) {
            richText = true,
          });

    static readonly Lazy<GUIStyle> _labelStyle =
        new(() =>
            new(GUI.skin.label) {
              padding = new(left: 5, right: 5, top: 5, bottom: 5)
            });

    static readonly Lazy<GUIStyle> _horizontalStyle =
        new(() =>
            new() {
              padding = new(left: 10, right: 10, top: 0, bottom: 0)
            });

    static readonly Lazy<GUIStyle> _buttonStyle =
        new(() =>
            new(GUI.skin.button) {
              padding = new(left: 10, right: 10, top: 5, bottom: 5)
            });

    static readonly Lazy<GUIStyle> _textFieldStyle =
        new(() =>
            new(GUI.skin.textField) {
              padding = new(left: 5, right: 5, top: 5, bottom: 5),
              wordWrap = false
            });
  }

  public static class StringListConfigEntryExtensions {
    public static StringListConfigEntry BindInOrder(
        this ConfigFile config, string section, string key, string description, string valuesSeparator) {
      return new StringListConfigEntry(config, section, key, description, valuesSeparator);
    }
  }
}
