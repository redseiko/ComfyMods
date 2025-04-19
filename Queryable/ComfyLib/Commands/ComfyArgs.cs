namespace ComfyLib;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public sealed class ComfyArgs {
  public static readonly Regex CommandRegex =
      new("^(?<command>\\w[\\w-]*)"
          + "(?:\\s+--"
              + "(?:(?<arg>\\w[\\w-]*)=(?:\"(?<value>[^\"]*?)\""
              + "|(?<value>\\S+))"
              + "|no(?<argfalse>\\w[\\w-]*)"
              + "|(?<argtrue>\\w[\\w-]*)))*");

  public static readonly char[] CommaSeparator = [','];

  public Terminal.ConsoleEventArgs Args { get; }
  public Terminal Context { get => Args.Context; }

  public string Command { get; private set; }
  public readonly Dictionary<string, string> ArgsValueByName = [];

  public ComfyArgs(Terminal.ConsoleEventArgs args) {
    Args = args;
    ParseArgs(args.FullLine);
  }

  void ParseArgs(string line) {
    Match match = CommandRegex.Match(line);
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

  public bool TryGetValue(string argName, out string argValue) {
    return ArgsValueByName.TryGetValue(argName, out argValue);
  }

  public bool TryGetValue(string argName, string argShortName, out string argValue) {
    return
        ArgsValueByName.TryGetValue(argName, out argValue)
        || ArgsValueByName.TryGetValue(argShortName, out argValue);
  }

  public bool TryGetValue<T>(string argName, out T argValue) {
    argValue = default;

    return
        ArgsValueByName.TryGetValue(argName, out string argStringValue)
        && argStringValue.TryParseValue(out argValue);
  }

  public bool TryGetValue<T>(string argName, string argShortName, out T argValue) {
    argValue = default;

    return
        (ArgsValueByName.TryGetValue(argName, out string argStringValue)
            || ArgsValueByName.TryGetValue(argShortName, out argStringValue))
        && argStringValue.TryParseValue(out argValue);
  }

  public bool TryGetListValue<T>(string argName, string argShortName, out List<T> argListValue) {
    if (!ArgsValueByName.TryGetValue(argName, out string argStringValue)
        && !ArgsValueByName.TryGetValue(argShortName, out argStringValue)) {
      argListValue = default;
      return false;
    }

    string[] values = argStringValue.Split(CommaSeparator, StringSplitOptions.RemoveEmptyEntries);
    argListValue = new(capacity: values.Length);

    for (int i = 0; i < values.Length; i++) {
      if (!values[i].TryParseValue(out T argValue)) {
        return false;
      }

      argListValue.Add(argValue);
    }

    return true;
  }

  public bool GetOptionalValue<T>(string argName, out T? argValue) {
    argValue = default;

    return
        !ArgsValueByName.TryGetValue(argName, out string argStringValue)
        || argStringValue.TryParseValue(out argValue);
  }

  public bool GetOptionalValue<T>(string argName, string argShortName, out T? argValue) {
    argValue = default;

    return
        (!ArgsValueByName.TryGetValue(argName, out string argStringValue)
            && !ArgsValueByName.TryGetValue(argShortName, out argStringValue))
        || argStringValue.TryParseValue(out argValue);
  }
}
