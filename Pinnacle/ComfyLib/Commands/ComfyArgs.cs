namespace ComfyLib;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public sealed class ComfyArgs {
  public static readonly Regex CommandRegex =
      new Regex(
          @"^(?<command>\w[\w-]*)"
              + @"(\s+--("
                  + @"(?<arg>\w[\w-]*)="
                      + @"(""(?<value>[^""]*?)"""
                      + @"|'(?<value>[^']*?)'"
                      + @"|(?<value>\S+))"
                  + @"|no(?<argfalse>\w[\w-]*)"
                  + @"|(?<argtrue>\w[\w-]*)))*",
          RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant,
          TimeSpan.FromMilliseconds(500));

  public static readonly char[] CommaSeparator = [','];

  public Terminal.ConsoleEventArgs Args { get; }
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
    int namesCount = names.Count;

    CaptureCollection values = match.Groups["value"].Captures;
    int valuesCount = values.Count;

    for (int i = 0; i < namesCount; i++) {
      ArgsValueByName[names[i].Value] = i < valuesCount ? values[i].Value : string.Empty;
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

  public bool TryGetListValue<T>(string argName, out List<T> argListValue) {
    if (!ArgsValueByName.TryGetValue(argName, out string argStringValue)) {
      argListValue = default;
      return false;
    }

    return GetListValue(argStringValue, out argListValue);
  }

  public bool TryGetListValue<T>(string argName, string argShortName, out List<T> argListValue) {
    if (!ArgsValueByName.TryGetValue(argName, out string argStringValue)
        && !ArgsValueByName.TryGetValue(argShortName, out argStringValue)) {
      argListValue = default;
      return false;
    }

    return GetListValue(argStringValue, out argListValue);
  }

  static bool GetListValue<T>(string argStringValue, out List<T> argListValue) {
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
