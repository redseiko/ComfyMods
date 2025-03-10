namespace Parrot;

using ComfyLib;

public static class SendParrotLogCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "send-parrot-log",
        "(Parrot) send-parrot-log --message=<message>",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    if (!args.TryGetValue("message", out string logMessage)) {
      return false;
    }

    ConnectionManager.SendParrotLog(logMessage);

    return true;
  }
}
