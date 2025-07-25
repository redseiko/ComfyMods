namespace Pinnacle;

using ComfyLib;

using UnityEngine;

public static class AddMapPinCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "add-map-pin",
        "(Pinnacle) add-map-pin [--position=<x,y,z>] [--pin-name=<string>] [--pin-type=<enum>] [--log]",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    Minimap minimap = Minimap.m_instance;
    Player player = Player.m_localPlayer;

    if (!minimap || !player) {
      return false;
    }

    Vector3 pinPosition = player.transform.position;

    if (args.TryGetValue("position", "pos", out string positionArg) && !positionArg.TryParseVector3(out pinPosition)) {
      Pinnacle.LogWarning($"Unable to parse --position: {positionArg}");
      return false;
    }

    if (!args.TryGetValue("pin-name", "name", out string pinName)) {
      pinName = string.Empty;
    }

    Minimap.PinType pinType = Minimap.PinType.Icon3;

    if (args.TryGetValue("pin-type", "type", out string pinTypeArg) && !pinTypeArg.TryParseValue(out pinType)) {
      Pinnacle.LogWarning($"Unable to parse --pin-type: {pinTypeArg}");
      return false;
    }

    Minimap.PinData pin = minimap.AddPin(pinPosition, pinType, pinName, save: true, isChecked: false);

    if (args.TryGetValue("log", out bool logPin) && logPin) {
      Pinnacle.LogInfo($"Added new pin... Position: {pin.m_pos:F0}, Type: {pinType:F}, Name: {pinName}");
    }

    return true;
  }
}
