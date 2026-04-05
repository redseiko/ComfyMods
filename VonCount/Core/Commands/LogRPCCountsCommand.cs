namespace VonCount;

using ComfyLib;

public static class LogRPCCountsCommand {
  public enum CountType {
    MethodHash,
    SenderId
  }

  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "log-rpc-counts",
        "(VonCount) log-rpc-counts --type=<MethodHash|SenderId>",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    if (!args.TryGetValue("type", "t", out CountType countType)) {
      countType = CountType.MethodHash;
    }

    switch (countType) {
      case CountType.MethodHash:
        CountManager.LogRPCCountsByMethodHash();
        break;

      case CountType.SenderId:
        CountManager.LogRPCCountsBySenderId();
        break;
    }
    
    return true;
  }
}
