namespace Queryable;

using System.Collections.Generic;
using System.IO;

using ComfyLib;

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
    if (!Directory.Exists(WorldUtils.WorldsDir)) {
      return [];
    }

    List<string> worldFilenames = [];

    foreach (string directory in Directory.GetDirectories(WorldUtils.WorldsDir)) {
      if (Directory.GetFiles(directory, "_main.*.db2").Length > 0) {
        worldFilenames.Add(Path.GetFileName(directory));
      }
    }

    return worldFilenames;
  }

  public static object Run(Terminal.ConsoleEventArgs args) {   
    try {
      ComfyLogger.PushContext(args.Context);

      if (args.Length <= 1) {
        ComfyLogger.LogError($"Missing filename arg.");
        return false;
      }

      string filenameArg = args[1];

      if (string.IsNullOrEmpty(filenameArg)) {
        ComfyLogger.LogError($"Missing or invalid filename arg.");
        return false;
      }

      ComfyLogger.LogInfo($"Querying world in file: {filenameArg}");

      return WorldUtils.QueryWorldFile(filenameArg);
    } finally {
      ComfyLogger.PopContext();
    }
  }
}
