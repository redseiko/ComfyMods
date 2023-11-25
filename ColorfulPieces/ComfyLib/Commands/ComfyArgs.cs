using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using UnityEngine;

namespace ComfyLib {
  public class ComfyArgs {
    public static readonly Regex CommandRegex =
        new("^(?<command>\\w[\\w-]*)"
            + "(?:\\s+--"
                + "(?:(?<arg>\\w[\\w-]*)=(?:\"(?<value>[^\"]*?)\""
                + "|(?<value>\\S+))"
                + "|no(?<argfalse>\\w[\\w-]*)"
                + "|(?<argtrue>\\w[\\w-]*)))*");

    public static readonly char[] CommaSeparator = { ',' };

    public Terminal.ConsoleEventArgs Args { get; }
    public string Command { get; private set; }
    public readonly Dictionary<string, string> ArgsValueByName = new();

    public ComfyArgs(Terminal.ConsoleEventArgs args) {
      Args = args;
      ParseArgs(Args);
    }

    void ParseArgs(Terminal.ConsoleEventArgs args) {
      Match match = CommandRegex.Match(args.FullLine);
      Command = match.Groups["command"].Value;

      foreach (Capture name in match.Groups["argtrue"].Captures) {
        ArgsValueByName[name.Value] = "true";
      }

      foreach (Capture name in match.Groups["argfalse"].Captures) {
        ArgsValueByName[name.Value] = "false";
      }

      CaptureCollection names = match.Groups["arg"].Captures;
      CaptureCollection values = match.Groups["value"].Captures;

      for (int i = 0; i < names.Count; i++) {
        ArgsValueByName[names[i].Value] = i < values.Count ? values[i].Value : string.Empty;
      }
    }

    public bool TryGetValue(string argName, string argShortName, out string argValue) {
      return
          ArgsValueByName.TryGetValue(argName, out argValue)
          || ArgsValueByName.TryGetValue(argShortName, out argValue);
    }

    public bool TryGetValue<T>(string argName, string argShortName, out T argValue) {
      argValue = default;

      return
          TryGetValue(argName, argShortName, out string argStringValue)
          && TryConvertValue(argName, argStringValue, out argValue);
    }

    public bool TryGetListValue<T>(string argName, string argShortName, out List<T> argListValue) {
      if (!TryGetValue(argName, argShortName, out string argStringValue)) {
        argListValue = default;
        return false;
      }

      string[] values = argStringValue.Split(CommaSeparator, StringSplitOptions.RemoveEmptyEntries);
      argListValue = new(capacity: values.Length);

      for (int i = 0; i < values.Length; i++) {
        if (!TryConvertValue(argName, values[i], out T argValue)) {
          return false;
        }

        argListValue.Add(argValue);
      }

      return true;
    }

    public static bool TryConvertValue<T>(string argName, string argStringValue, out T argValue) {
      try {
        if (typeof(T) == typeof(string)) {
          argValue = (T) (object) argStringValue;
        } else if (typeof(T) == typeof(Vector2)) {
          argValue = (T) (object) ParseVector2(argStringValue);
        } else if (typeof(T) == typeof(Vector3)) {
          argValue = (T) (object) ParseVector3(argStringValue);
        } else {
          argValue = (T) Convert.ChangeType(argStringValue, typeof(T));
        }

        return true;
      } catch (Exception exception) {
        Debug.LogError($"Failed to convert arg '{argName}' value '{argStringValue}' to type {typeof(T)}: {exception}");
      }

      argValue = default;
      return false;
    }

    public static Vector2 ParseVector2(string text) {
      string[] parts = text.Split(CommaSeparator, 2, StringSplitOptions.RemoveEmptyEntries);

      if (parts.Length == 2) {
        return new(
            float.Parse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture),
            float.Parse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture));
      }

      throw new InvalidOperationException($"Could not parse {text} as Vector2.");
    }

    public static Vector3 ParseVector3(string text) {
      string[] parts = text.Split(CommaSeparator, 3, StringSplitOptions.RemoveEmptyEntries);

      if (parts.Length == 3
          && float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x)
          && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y)
          && float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z)) {
        return new(x, y, z);
      }

      throw new InvalidOperationException($"Could not parse {text} as Vector3.");
    }
  }
}
