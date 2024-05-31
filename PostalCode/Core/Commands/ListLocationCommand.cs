namespace PostalCode;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;

public static class ListLocationCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "list-location",
        "(PostalCode) list-location --prefab=<string>",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    if (!args.TryGetValue("prefab", "p", out string prefabName)) {
      PostalCode.LogError($"Missing or invalid value for --prefab arg.");
      return false;
    }

    ZoneSystem zoneSystem = ZoneSystem.m_instance;
    List<Vector3> positions = [];

    foreach (ZoneSystem.LocationInstance locationInstance in zoneSystem.m_locationInstances.Values) {
      if (locationInstance.m_location.m_prefab.Name == prefabName) {
        positions.Add(locationInstance.m_position);
      }
    }

    PostalCode.LogInfo(
        $"Found {positions.Count} locations matching prefab: {prefabName}\n"
            + string.Join("\n", positions));

    return true;
  }
}
