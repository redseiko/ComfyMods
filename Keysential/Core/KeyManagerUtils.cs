using System;
using System.Collections.Generic;
using System.IO;

namespace Keysential {
  public static class KeyManagerUtils {
    public static readonly SyncedList StartUpKeyManagerList =
        new(
            Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), "keysential-startup.txt"),
            "KeyManager (commands) to execute at start-up.");

    public static void RunStartUpKeyManagers() {
      foreach (string command in StartUpKeyManagerList.GetList()) {
        Console.instance.TryRunCommand(command, silentFail: false, skipAllowedCheck: true);
      }
    }

    public static void AddStartUpKeyManager(string command) {
      Keysential.LogInfo($"Adding KeyManager command to start-up list: {command}");
      StartUpKeyManagerList.Add(command);
    }

    public static bool RemoveStartUpKeyManager(string managerId) {
      List<string> commands = StartUpKeyManagerList.GetList();

      for (int i = commands.Count - 1; i >= 0; i--) {
        if (IsMatchingCommand(managerId, commands[i])) {
          Keysential.LogInfo($"Removing KeyManager command from start-up list: {commands[i]}");

          commands.RemoveAt(i);
          StartUpKeyManagerList.Save();

          return true;
        }
      }

      return false;
    }

    static readonly char[] _spaceSeparator = new char[] { ' ' };

    static bool IsMatchingCommand(string managerId, string command) {
      string[] parts = command.Split(_spaceSeparator, StringSplitOptions.RemoveEmptyEntries);
      return parts.Length >= 2 && parts[1] == managerId;
    }
  }
}
