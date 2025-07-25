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

  public bool TryGetStringValues(string argName, string argShortName, out string[] values) {
    if (!ArgsValueByName.TryGetValue(argName, out string argStringValue)
        && !ArgsValueByName.TryGetValue(argShortName, out argStringValue)) {
      values = [];
      return false;
    }

    values = argStringValue.Split(CommaSeparator, StringSplitOptions.RemoveEmptyEntries);
    return true;
  }
}
