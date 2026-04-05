namespace VonCount;

using ComfyLib;

public static class LogRPCCountsCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "log-rpc-counts",
        "(VonCount) log-rpc-counts",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    CountManager.LogRPCCounts();
    return true;
  }
}
