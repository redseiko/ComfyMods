namespace Queryable;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using ComfyLib;

using UnityEngine;

public static class QueryWorldCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "query-world",
        "(Queryable) query-world",
        Run,
        optionsFetcher: GetWorldFilenames,
        alwaysRefreshTabOptions: true);
  }

  public static List<string> GetWorldFilenames() {
    if (Directory.Exists(WorldUtils.WorldsDir)) {
      return [.. Directory.GetFiles(WorldUtils.WorldsDir, "*.db").Select(Path.GetFileName)];
    }

    return [];
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    if (args.Length <= 1) {
      ComfyLogger.LogError($"Missing filename arg.");
      return false;
    }

    string filenameArg = args[1];

    if (string.IsNullOrEmpty(filenameArg) || filenameArg.Length <= 3) {
      ComfyLogger.LogError($"Missing or invalid filename arg.");
      return false;
    }

    args.Args[1] = $"--world={filenameArg}";
    args.FullLine = string.Join(" ", args.Args);

    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    ComfyLogger.LogInfo($"World: {args.ArgsValueByName["world"]}", args.Context);

    // TODO: redseiko@ - implement the rest of the command.

    return true;
  }
}
