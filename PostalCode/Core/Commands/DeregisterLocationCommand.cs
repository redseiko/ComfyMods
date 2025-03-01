namespace PostalCode;

using ComfyLib;

using UnityEngine;

public static class DeregisterLocationCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "deregister-location",
        "(PostalCode) deregister-location --position=<x,y,z>",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    if (!args.TryGetValue("position", "pos", out Vector3 position)) {
      PostalCode.LogError($"Missing or invalid value for --position arg.");
      return false;
    }

    ZoneSystem zoneSystem = ZoneSystem.m_instance;
    Vector2i sector = ZoneSystem.GetZone(position);

    if (zoneSystem.m_locationInstances.TryGetValue(sector, out ZoneSystem.LocationInstance instance)) {
      PostalCode.LogInfo(
          $"Deregistered existing location {instance.m_location.m_prefabName} at "
              + $"sector: {sector:F0}, position: {position:F0}, generated: {instance.m_placed}.");

      zoneSystem.m_locationInstances.Remove(sector);
    } else {
      PostalCode.LogInfo($"No location was found at sector {sector:F0}, position: {position:F0}.");
    }

    return true;
  }
}
