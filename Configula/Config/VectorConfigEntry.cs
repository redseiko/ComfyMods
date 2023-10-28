using System.Collections.Generic;
using System.Globalization;

using ConfigurationManager;

using UnityEngine;

namespace Configula {
  public static class VectorConfigEntry {
    public class FloatInputField {
      public string FieldLabel { get; set; }
      public float CurrentValue { get; set; } = 0f;
      public string CurrentText { get; set; } = string.Empty;
      public Color CurrentColor { get; set; } = GUI.color;

      public FloatInputField(string label) {
        FieldLabel = label;
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
        string textValue = GUILayout.TextField(CurrentText, GUILayout.MaxWidth(100f), GUILayout.ExpandWidth(true));
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
    }

    sealed class Vector2ConfigCacheEntry {
      public Vector2 Value = Vector2.zero;

      public string FieldTextX = string.Empty;
      public Color FieldColorX = Color.clear;

      public string FieldTextY = string.Empty;
      public Color FieldColorY = Color.clear;
    }

    static readonly Dictionary<SettingEntryBase, Vector2ConfigCacheEntry> _vector2ConfigCache = new();

    public static void DrawVector2(SettingEntryBase configEntry) {
      Vector2 configValue = (Vector2) configEntry.Get();

      if (!_vector2ConfigCache.TryGetValue(configEntry, out Vector2ConfigCacheEntry cacheEntry)) {
        cacheEntry = new() {
          Value = configValue,
          FieldColorX = GUI.color,
          FieldColorY = GUI.contentColor,
        };

        _vector2ConfigCache[configEntry] = cacheEntry;
      }

      if (GUIFocus.HasChanged() || GUIHelper.IsEnterPressed() || cacheEntry.Value != configValue) {
        cacheEntry.Value = configValue;

        cacheEntry.FieldTextX = configValue.x.ToString(NumberFormatInfo.InvariantInfo);
        cacheEntry.FieldColorX = GUI.color;

        cacheEntry.FieldTextY = configValue.y.ToString(NumberFormatInfo.InvariantInfo);
        cacheEntry.FieldColorY = GUI.color;
      }

      GUILayout.Label("X", GUILayout.ExpandWidth(false));

      GUIHelper.BeginColor(cacheEntry.FieldColorX);
      string fieldTextX = GUILayout.TextField(cacheEntry.FieldTextX, GUILayout.ExpandWidth(true));
      GUIHelper.EndColor();

      GUILayout.Label("Y", GUILayout.ExpandWidth(false));

      GUIHelper.BeginColor(cacheEntry.FieldColorY);
      string fieldTextY = GUILayout.TextField(cacheEntry.FieldTextY, GUILayout.ExpandWidth(true));
      GUIHelper.EndColor();

      if (cacheEntry.FieldTextX != fieldTextX) {
        cacheEntry.FieldTextX = fieldTextX;

        if (ShouldParse(fieldTextX)
            && float.TryParse(fieldTextX, NumberStyles.Float, CultureInfo.InvariantCulture, out float x)) {
          Vector2 result = new(x, configValue.y);
          configEntry.Set(result);
          cacheEntry.Value = (Vector2) configEntry.Get();
          cacheEntry.FieldTextX = cacheEntry.Value.x.ToString(NumberFormatInfo.InvariantInfo);
          cacheEntry.FieldColorX = GUI.color;
        } else {
          cacheEntry.FieldColorX = Color.red;
        }
      } else if (cacheEntry.FieldTextY != fieldTextY) {
        cacheEntry.FieldTextY = fieldTextY;

        if (ShouldParse(fieldTextY)
            && float.TryParse(fieldTextY, NumberStyles.Float, CultureInfo.InvariantCulture, out float y)) {
          Vector2 result = new(configValue.x, y);
          configEntry.Set(result);
          cacheEntry.Value = (Vector2) configEntry.Get();
          cacheEntry.FieldTextY = cacheEntry.Value.y.ToString(NumberFormatInfo.InvariantInfo);
          cacheEntry.FieldColorY = GUI.color;
        } else {
          cacheEntry.FieldColorY = Color.red;
        }
      } else {
        return;
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
