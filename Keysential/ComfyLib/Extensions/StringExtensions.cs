namespace ComfyLib;

using System;
using System.Globalization;

using UnityEngine;

public static class StringExtensions {
  public static readonly char[] CommaSeparator = { ',' };

  public static bool TryParseVector(this string text, out Vector2i vector) {
    string[] parts = text.Split(CommaSeparator, 2, StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length == 2
        && int.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out int x)
        && int.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out int y)) {
      vector = new(x, y);
      return true;
    }

    vector = default;
    return false;
  }

  public static bool TryParseVector(this string text, out Vector3 vector) {
    string[] parts = text.Split(CommaSeparator, 3, StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length == 3
        && float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x)
        && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y)
        && float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z)) {
      vector = new(x, y, z);
      return true;
    }

    vector = default;
    return false;
  }

  public static string[] GetEncodedGlobalKeys(this string text) {
    string[] keys = text.Split(CommaSeparator, StringSplitOptions.RemoveEmptyEntries);

    for (int i = 0; i < keys.Length; i++) {
      keys[i] = keys[i].Replace('=', ' ');
    }

    return keys;
  }
}
