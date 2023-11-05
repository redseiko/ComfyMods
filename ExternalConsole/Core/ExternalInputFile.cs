using System;
using System.Collections;
using System.Globalization;

using UnityEngine;

namespace ExternalConsole {
  public static class ExternalInputFile {
    public static IEnumerator ReadFromFileCoroutine(string externalInputFilename) {
      ExternalConsole.LogInfo($"Starting to read external input from: {externalInputFilename}");

      Console console = Console.m_instance;
      WaitForSeconds waitInterval = new(seconds: 1f);
      SyncedList externalInput = new(externalInputFilename, $"ExternalConsole: {externalInputFilename}");

      while (console) {
        externalInput.Load();

        if (externalInput.m_list.Count > 0) {
          foreach (string line in externalInput.m_list) {
            ExternalConsole.LogInfo($"Processing external input: {line}");
            console.TryRunCommand(line, silentFail: false, skipAllowedCheck: true);

            externalInput.m_comments.Add(
                $"// [{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)}] {line}");
          }

          externalInput.m_list.Clear();
          externalInput.Save();
        }

        yield return waitInterval;
      }
    }
  }
}
