namespace ExternalConsole;

using System.Collections;
using System.Globalization;
using System.IO;

using UnityEngine;

using static PluginConfig;

public static class ConsoleManager {
  public static void SetupExternalInput(Console console) {
    string filename = ExternalInputFilename.Value;

    if (string.IsNullOrEmpty(filename)) {
      return;
    }

    console.StartCoroutine(
        ProcessCommandFromFile(Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), filename)));
  }

  public static IEnumerator ProcessCommandFromFile(string filename) {
    ExternalConsole.LogInfo($"Processing commands from file: {filename}");

    Console console = Console.m_instance;
    WaitForSeconds waitInterval = new(seconds: 3f);
    SyncedList commandList = new(new(FileHelpers.FileSource.Local, filename), $"ExternalConsole: {filename}");

    while (console) {
      ProcessCommandList(console, commandList);
      yield return waitInterval;
    }
  }

  static void ProcessCommandList(Console console, SyncedList commandList) {
    commandList.Load();

    if (commandList.m_list.Count <= 0) {
      return;
    }

    using StreamWriter commandLog = GetCommandLog(CommandLogFilename.Value);

    foreach (string line in commandList.m_list) {
      try {
        ExternalConsole.LogInfo($"Processing command line: {line}");
        console.TryRunCommand(line, silentFail: false, skipAllowedCheck: true);
      } catch (System.Exception exception) {
        ExternalConsole.LogError($"Failed processing command line due to exception: {exception}");
      }

      commandLog.WriteLine(
          $"[{System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)}] {line}");
    }

    commandList.m_list.Clear();
    commandList.Save();
  }

  static StreamWriter GetCommandLog(string filename) {
    return new StreamWriter(
        Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), filename),
        append: true,
        encoding: System.Text.Encoding.UTF8) {
      AutoFlush = true
    };
  }
}
