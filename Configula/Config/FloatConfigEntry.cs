using System.Collections.Generic;
using System.Globalization;

using ConfigurationManager;

using UnityEngine;

namespace Configula {
  public static class FloatConfigEntry {
    sealed class FloatConfigCacheEntry {
      public float Value = 0f;
      public string FieldText = string.Empty;
      public Color FieldColor = Color.clear;
    }

    static readonly Dictionary<SettingEntryBase, FloatConfigCacheEntry> _floatConfigCache = new();

    public static void DrawFloat(SettingEntryBase configEntry) {
      float configValue = (float) configEntry.Get();

      if (!_floatConfigCache.TryGetValue(configEntry, out FloatConfigCacheEntry cacheEntry)) {
        cacheEntry = new() {
          Value = configValue,
          FieldColor = GUI.color,
        };

        _floatConfigCache[configEntry] = cacheEntry;
      }

      if (GUIFocus.HasChanged() || GUIHelper.IsEnterPressed() || cacheEntry.Value != configValue) {
        cacheEntry.Value = configValue;
        cacheEntry.FieldText = configValue.ToString(NumberFormatInfo.InvariantInfo);
        cacheEntry.FieldColor = GUI.color;
      }

      GUIHelper.BeginColor(cacheEntry.FieldColor);
      string textValue = GUILayout.TextArea(cacheEntry.FieldText, GUILayout.ExpandWidth(true));
      GUIHelper.EndColor();

      if (textValue == cacheEntry.FieldText) {
        return;
      }

      cacheEntry.FieldText = textValue;

      if (ShouldParse(textValue)
          && float.TryParse(textValue, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out float result)) {
        configEntry.Set(result);
        cacheEntry.Value = (float) configEntry.Get();
        cacheEntry.FieldText = cacheEntry.Value.ToString(NumberFormatInfo.InvariantInfo);

        if (cacheEntry.FieldText == textValue) {
          cacheEntry.FieldColor = GUI.color;
        } else {
          cacheEntry.FieldColor = Color.yellow;
          cacheEntry.FieldText = textValue;
        }
      } else {
        cacheEntry.FieldColor = Color.red;
      }
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
}
