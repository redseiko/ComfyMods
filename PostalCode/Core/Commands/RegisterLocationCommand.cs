namespace PostalCode;

using ComfyLib;

using UnityEngine;

public static class RegisterLocationCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "register-location",
        "(PostalCode) register-location --prefab=<string> --position=<x,y,z> --generated=<bool>",
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

    if (!zoneSystem.m_locationsByHash.TryGetValue(
            prefabName.GetStableHashCode(), out ZoneSystem.ZoneLocation zoneLocation)) {
      PostalCode.LogError($"Could not find matching ZoneLocation for prefab: {prefabName}");
      return false;
    }

    if (!args.TryGetValue("position", "pos", out Vector3 position)) {
      PostalCode.LogError($"Missing or invalid value for --position arg.");
      return false;
    }

    if (!args.TryGetValue("generated", "g", out bool generated)) {
      PostalCode.LogError($"Missing or invalid value for --generated arg.");
      return false;
    }

    Vector2i sector = zoneSystem.GetZone(position);

    if (zoneSystem.m_locationInstances.TryGetValue(sector, out ZoneSystem.LocationInstance instance)) {
      PostalCode.LogInfo(
          $"Replacing existing location {instance.m_location.m_prefabName} at "
              + $"sector: {sector:F0}, position: {position:F0}, generated: {instance.m_placed}.");
    }

    zoneSystem.m_locationInstances[sector] = new() {
      m_location = zoneLocation,
      m_position = position,
      m_placed = generated,
    };

    PostalCode.LogInfo(
        $"Registered location {prefabName} at sector: {sector}, position: {position}, generated: {generated}.");

    return true;
  }
}
