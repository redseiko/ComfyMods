using System.Collections.Generic;

using ComfyLib;

using UnityEngine;

namespace Transporter {
  public static class TeleportPlayerCommand {
    [ComfyCommand]
    public static IEnumerable<Terminal.ConsoleCommand> Register() {
      yield return new Terminal.ConsoleCommand(
          "teleport-player",
          "(Transporter) teleport-player",
          args => Run(new ComfyArgs(args)));
    }

    public enum TeleportTrigger {
      Now,
      Login,
    }

    public static bool Run(ComfyArgs comfyArgs) {
      if (!comfyArgs.TryGetListValue("player-id", "pid", out List<long> playerIds) || playerIds.Count <= 0) {
        Transporter.LogError($"Missing or invalid arg: --playerId");
        return false;
      }

      if (!comfyArgs.TryGetValue("destination", "d", out Vector3 destination)) {
        Transporter.LogError($"Missing or invalid arg: --destination");
        return false;
      }

      if (!TryGetTeleportTrigger(comfyArgs, out TeleportTrigger trigger)) {
        Transporter.LogError($"Missing or invalid arg: --trigger");
        return false;
      }

      if (trigger == TeleportTrigger.Now) {
        return TeleportNow(playerIds, destination);
      } else if (trigger == TeleportTrigger.Login) {
        // TODO: top-kek.
      }
      
      return true;
    }

    public static bool TeleportNow(List<long> playerIds, Vector3 destination) {
      Transporter.LogInfo($"TeleportNow ... playerIds: {string.Join(", ", playerIds)}");
      List<ZDO> playerZDOs = PlayerUtils.GetPlayerZDOs(playerIds);

      if (playerZDOs.Count <= 0) {
        Transporter.LogInfo($"No matching player ZDOs found.");
        return true;
      }

      ZRoutedRpc routedRpc = ZRoutedRpc.s_instance;

      foreach (ZDO zdo in playerZDOs) {
        Transporter.LogInfo($"Teleporting player ({zdo.m_uid}) from {zdo.m_position:F0} to {destination:F0}");
        routedRpc.InvokeRoutedRPC(zdo.GetOwner(), zdo.m_uid, "RPC_TeleportTo", destination, Quaternion.identity, true);
      }

      return true;
    }

    static bool TryGetTeleportTrigger(ComfyArgs comfyArgs, out TeleportTrigger trigger) {
      if (comfyArgs.TryGetValue("trigger", "t", out trigger)) {
        return true;
      } else if (comfyArgs.TryGetValue("now", out bool _)) {
        trigger = TeleportTrigger.Now;
        return true;
      } else if (comfyArgs.TryGetValue("login", out bool _)) {
        trigger = TeleportTrigger.Login;
        return true;
      }

      return false;
    }
  }
}
