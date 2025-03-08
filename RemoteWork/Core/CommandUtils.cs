namespace RemoteWork;

public static class CommandUtils {
  public static readonly char[] SpaceSeparator = [' '];

  public static void RunCommand(ZRpc rpc, string command) {
    string[] parts = command.Split(SpaceSeparator, 2, System.StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length >= 1
        && !string.IsNullOrEmpty(parts[0])
        && Terminal.commands.TryGetValue(parts[0].ToLowerInvariant(), out Terminal.ConsoleCommand consoleCommand)) {
      RunCommand(rpc, consoleCommand, new Terminal.ConsoleEventArgs(command, Console.instance));
    } else {
      rpc.RemotePrint($"Invalid command: {command}");
    }
  }

  static void RunCommand(ZRpc rpc, Terminal.ConsoleCommand command, Terminal.ConsoleEventArgs args) {
    rpc.RemotePrint($"Running command: {args.FullLine}");

    try {
      if (command.action != null) {
        command.action(args);
        rpc.RemotePrint($"Command completed.");

        return;
      }

      object result = command.actionFailable(args);

      if (result is bool resultAsBool && !resultAsBool) {
        rpc.RemotePrint($"Command failed!");
      } else if (result is string resultAsString && resultAsString != null) {
        rpc.RemotePrint($"Command failed with error: {resultAsString}");
      } else {
        rpc.RemotePrint($"Command successful!");
      }
    } catch (System.Exception exception) {
      rpc.RemotePrint($"Command failed with exception: {exception}");
    }
  }

  public static readonly int RemotePrintHashCode = "RemotePrint".GetStableHashCode();

  static void RemotePrint(this ZRpc rpc, string message) {
    if (rpc == null || !rpc.m_socket.IsConnected()) {
      return;
    }

    ZPackage package = rpc.m_pkg;

    package.Clear();
    package.Write(RemotePrintHashCode);
    package.Write($"[RemoteWork] {message}");

    rpc.SendPackage(package);
  }
}
