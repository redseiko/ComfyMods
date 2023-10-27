using System.Collections.Generic;
using System.Globalization;

using ConfigurationManager;

using UnityEngine;

namespace Configula {
  public static class VectorConfigEntry {
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

        cacheEntry.FieldTextX = configValue.x.ToString("F", NumberFormatInfo.InvariantInfo);
        cacheEntry.FieldColorX = GUI.color;

        cacheEntry.FieldTextY = configValue.y.ToString("F", NumberFormatInfo.InvariantInfo);
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

      if (fieldTextX == cacheEntry.FieldTextX && fieldTextY == cacheEntry.FieldTextY) {
        return;
      }

      cacheEntry.FieldTextX = fieldTextX;
      cacheEntry.FieldTextY = fieldTextY;

      Vector2 result = Vector2.zero;

      bool isValidX =
          ShouldParse(fieldTextX)
          && float.TryParse(fieldTextX, NumberStyles.Any, CultureInfo.InvariantCulture, out result.x);

      bool isValidY =
          ShouldParse(fieldTextY)
          && float.TryParse(fieldTextY, NumberStyles.Any, CultureInfo.InvariantCulture, out result.y);

      if (isValidX && isValidX) {
        configEntry.Set(result);
      }

      cacheEntry.FieldColorX = isValidX ? GUI.color : Color.red;
      cacheEntry.FieldColorY = isValidY ? GUI.color : Color.red;
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
