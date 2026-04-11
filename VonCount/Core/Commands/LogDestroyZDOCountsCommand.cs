namespace VonCount;

using ComfyLib;

public static class LogDestroyZDOCountsCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "log-destroy-zdo-counts",
        "(VonCount) log-destroy-zdo-counts [--limit=<0>] [--min=<0>]",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    if (!args.TryGetValue("limit", out int limitArg)) {
      limitArg = 0;
    }

    if (!args.TryGetValue("min", out int minimumArg)) {
      minimumArg = 0;
    }

    CountManager.LogDestroyZDOCounts(limitArg, minimumArg);
    return true;
  }
}
