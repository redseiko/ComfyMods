namespace Atlas;

using System.Collections.Generic;

using ComfyLib;

public static class DeleteZDOsCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "delete-zdos",
        "(Atlas) delete-zdos --prefab=<name1,name2> [--dry-run=<true|false>]",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    if (!args.TryGetListValue("prefab", "p", out List<string> prefabNames) || prefabNames.Count <= 0) {
      PluginLogger.LogError("Missing or invalid arg: --prefab");
      return false;
    }

    if (!args.TryGetValue("dry-run", out bool isDryRun)) {
      isDryRun = true;
    }

    DeleteMatchingZDOs(prefabNames, isDryRun);

    return true;
  }

  public static void DeleteMatchingZDOs(List<string> prefabNames, bool isDryRun) {
    PluginLogger.LogInfo($"Finding ZDOs matching prefab: {string.Join(", ", prefabNames)}");

    List<ZDOID> matchingZDOIDs = FindMatchingZDOIDs(prefabNames);
    PluginLogger.LogInfo($"Found {matchingZDOIDs.Count} matching ZDOs.");

    if (matchingZDOIDs.Count <= 0) {
      return;
    }

    if (isDryRun) {
      PluginLogger.LogInfo($"In dry-run mode, not deleting {matchingZDOIDs.Count} ZDOs.");
    } else {
      PluginLogger.LogInfo($"Deleting {matchingZDOIDs.Count} ZDOs using DestroySendList.");
      ZDOMan.s_instance.m_destroySendList.AddRange(matchingZDOIDs);
    }
  }

  static List<ZDOID> FindMatchingZDOIDs(List<string> prefabNames) {
    HashSet<int> prefabHashes = new(capacity: prefabNames.Count);

    foreach (string name in prefabNames) {
      prefabHashes.Add(name.GetStableHashCode());
    }

    List<ZDOID> matchingZDOIDs = new(capacity: 1024);

    foreach (ZDO zdo in ZDOMan.s_instance.m_objectsByID.Values) {
      if (prefabHashes.Contains(zdo.m_prefab)) {
        matchingZDOIDs.Add(zdo.m_uid);
      }
    }

    return matchingZDOIDs;
  }
}
