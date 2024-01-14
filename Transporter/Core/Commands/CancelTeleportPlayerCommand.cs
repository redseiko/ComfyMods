using System.Collections.Generic;

using ComfyLib;

namespace Transporter {
  public static class CancelTeleportPlayerCommand {
    [ComfyCommand]
    public static IEnumerable<Terminal.ConsoleCommand> Register() {
      yield return new Terminal.ConsoleCommand(
          "cancel-teleport-player",
          "(Transporter) cancel-teleport-player",
          args => Run(new ComfyArgs(args)));
    }

    public static bool Run(ComfyArgs comfyArgs) {
      if (!comfyArgs.TryGetListValue("player-id", "pid", out List<long> playerIds) || playerIds.Count <= 0) {
        Transporter.LogError($"Missing or invalid arg: --playerId");
        return false;
      }

      foreach (long playerId in playerIds) {
        TeleportManager.CancelTeleportPlayer(playerId);
      }

      return true;
    }
  }
}
