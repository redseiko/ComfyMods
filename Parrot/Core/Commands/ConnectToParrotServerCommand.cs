namespace Parrot;

using ComfyLib;

public static class ConnectToParrotServerCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "connect-to-parrot-server",
        "(Parrot) connect-to-parrot-server --host=<host> --port=<port>",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    if (!args.TryGetValue("host", out string serverHost)) {
      return false;
    }

    if (!args.TryGetValue("port", out int serverPort)) {
      return false;
    }

    return ConnectionManager.ConnectToParrotServer(serverHost, serverPort);
  }
}
