namespace Keysential;

using ComfyLib;

public static class StopKeyManagerCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "stopkeymanager",
        "stopkeymanager <id: id1> [remove: true/false]",
        args => Run(args));
  }

  public static bool Run(Terminal.ConsoleEventArgs args) {
    if (args.Length < 2) {
      Keysential.LogError($"Not enough args for stopkeymanager command.");
      return false;
    }

    if (args.Length >= 3 && bool.TryParse(args[2], out bool removeFromStartup) && removeFromStartup) {
      KeyManagerUtils.RemoveStartUpKeyManager(args[1]);
    }

    return GlobalKeysManager.StopKeyManager(args[1]);
  }
}
