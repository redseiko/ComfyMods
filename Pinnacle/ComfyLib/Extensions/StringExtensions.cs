﻿namespace ComfyLib;

using System;
using System.Globalization;

using UnityEngine;

public static class StringExtensions {
  public static readonly char[] CommaSeparator = [','];
  public static readonly char[] ColonSeparator = [':'];

  public static bool TryParseValue<T>(this string text, out T value) {
    try {
      if (typeof(T) == typeof(string)) {
        value = (T) (object) text;
      } else if (typeof(T) == typeof(Vector2) && text.TryParseVector2(out Vector2 valueVector2)) {
        value = (T) (object) valueVector2;
      } else if (typeof(T) == typeof(Vector3) && text.TryParseVector3(out Vector3 valueVector3)) {
        value = (T) (object) valueVector3;
      } else if (typeof(T) == typeof(ZDOID) && text.TryParseZDOID(out ZDOID valueZDOID)) {
        value = (T) (object) valueZDOID;
      } else if (typeof(T).IsEnum) {
        value = (T) Enum.Parse(typeof(T), text, ignoreCase: true);
      } else {
        value = (T) Convert.ChangeType(text, typeof(T));
      }

      return true;
    } catch (Exception exception) {
      Debug.LogError($"Failed to convert value '{text}' to type {typeof(T)}: {exception}");
    }

    value = default;
    return false;
  }

  public static bool TryParseVector2(this string text, out Vector2 value) {
    string[] parts = text.Split(CommaSeparator, 2, StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length == 2
        && float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x)
        && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y)) {
      value = new(x, y);
      return true;
    }

    value = default;
    return false;
  }

  public static bool TryParseVector3(this string text, out Vector3 value) {
    string[] parts = text.Split(CommaSeparator, 3, StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length == 3
        && float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x)
        && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y)
        && float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z)) {
      value = new(x, y, z);
      return true;
    }

    value = default;
    return false;
  }

  public static bool TryParseZDOID(this string text, out ZDOID value) {
    string[] parts = text.Split(ColonSeparator, 2, StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length == 2
        && long.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out long userId)
        && uint.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out uint id)) {
      value = new(userId, id);
      return true;
    }

    value = default;
    return false;
  }
}
