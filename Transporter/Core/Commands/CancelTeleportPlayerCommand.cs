namespace Transporter;

using System.Collections.Generic;

using ComfyLib;

public static class CancelTeleportPlayerCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "cancel-teleport-player",
        "(Transporter) cancel-teleport-player --player-id",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    if (!args.TryGetListValue("player-id", "pid", out List<long> playerIds) || playerIds.Count <= 0) {
      Transporter.LogError($"Missing or invalid arg: --player-id");
      return false;
    }

    foreach (long playerId in playerIds) {
      TeleportManager.CancelTeleportPlayer(playerId);
    }

    return true;
  }
}
