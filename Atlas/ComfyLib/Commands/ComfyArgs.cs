namespace ComfyLib;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

public sealed class ComfyArgs {
  public static readonly Regex CommandRegex =
      new("^(?<command>\\w[\\w-]*)"
          + "(?:\\s+--"
              + "(?:(?<arg>\\w[\\w-]*)=(?:\"(?<value>[^\"]*?)\""
              + "|(?<value>\\S+))"
              + "|no(?<argfalse>\\w[\\w-]*)"
              + "|(?<argtrue>\\w[\\w-]*)))*");

  public static readonly char[] CommaSeparator = { ',' };

  public readonly Terminal.ConsoleEventArgs Args;
  public readonly string Command;
  public readonly Dictionary<string, string> ArgsValueByName;

  public ComfyArgs(Terminal.ConsoleEventArgs args) {
    Args = args;
    ParseArgs(Args, out Command, out ArgsValueByName);
  }

  static void ParseArgs(
      Terminal.ConsoleEventArgs args, out string command, out Dictionary<string, string> argsValueByName) {
    argsValueByName = new();

    Match match = CommandRegex.Match(args.FullLine);
    command = match.Groups["command"].Value;

    foreach (Capture name in match.Groups["argtrue"].Captures) {
      argsValueByName[name.Value] = "true";
    }

    foreach (Capture name in match.Groups["argfalse"].Captures) {
      argsValueByName[name.Value] = "false";
    }

    CaptureCollection names = match.Groups["arg"].Captures;
    CaptureCollection values = match.Groups["value"].Captures;

    for (int i = 0; i < names.Count; i++) {
      argsValueByName[names[i].Value] = i < values.Count ? values[i].Value : string.Empty;
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
        && TryConvertValue(argStringValue, out argValue);
  }

  public bool TryGetListValue<T>(string argName, string argShortName, out List<T> argListValue) {
    if (!TryGetValue(argName, argShortName, out string argStringValue)) {
      argListValue = default;
      return false;
    }

    string[] values = argStringValue.Split(CommaSeparator, StringSplitOptions.RemoveEmptyEntries);
    argListValue = new(capacity: values.Length);

    for (int i = 0; i < values.Length; i++) {
      if (!TryConvertValue(values[i], out T argValue)) {
        return false;
      }

      argListValue.Add(argValue);
    }

    return true;
  }

  public static bool TryConvertValue<T>(string argStringValue, out T argValue) {
    try {
      if (typeof(T) == typeof(string)) {
        argValue = (T) (object) argStringValue;
      } else if (typeof(T) == typeof(Vector2) && argStringValue.TryParseVector2(out Vector2 argValueVector2)) {
        argValue = (T) (object) argValueVector2;
      } else if (typeof(T) == typeof(Vector3) && argStringValue.TryParseVector3(out Vector3 argValueVector3)) {
        argValue = (T) (object) argValueVector3;
      } else if (typeof(T) == typeof(ZDOID) && argStringValue.TryParseZDOID(out ZDOID argValueZDOID)) {
        argValue = (T) (object) argValueZDOID;
      } else {
        argValue = (T) Convert.ChangeType(argStringValue, typeof(T));
      }

      return true;
    } catch (Exception exception) {
      Debug.LogError($"Failed to convert value '{argStringValue}' to type {typeof(T)}: {exception}");
    }

    argValue = default;
    return false;
  }
}
