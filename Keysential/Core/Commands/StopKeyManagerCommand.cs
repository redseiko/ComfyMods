namespace Keysential;

using ComfyLib;

public static class StopKeyManagerCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "stop-key-manager",
        "stop-key-manager --id=<manager-id> [--remove]",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    if (!args.TryGetValue("id", out string managerId)) {
      Keysential.LogError($"Missing string value for: --id");
      return false;
    }

    if (!args.TryGetValue("remove", out bool removeFromStartUp)) {
      removeFromStartUp = false;
    }

    if (removeFromStartUp) {
      KeyManagerUtils.RemoveStartUpKeyManager(managerId);
    }

    return GlobalKeysManager.StopKeyManager(managerId);
  }
}
