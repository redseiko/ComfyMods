namespace Transporter;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;

public static class TeleportPlayerCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "teleport-player",
        "(Transporter) teleport-player --player-id=<123> --destination=<x,y,z>",
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

    if (!args.TryGetValue("destination", "d", out Vector3 destination)) {
      Transporter.LogError($"Missing or invalid arg: --destination");
      return false;
    }

    foreach (long playerId in playerIds) {
      TeleportManager.TeleportPlayer(playerId, destination);
    }
    
    return true;
  }
}
