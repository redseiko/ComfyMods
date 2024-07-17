namespace ColorfulPieces;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;

public static class ClearColorCommand {
  [ComfyCommand]
  public static IEnumerable<Terminal.ConsoleCommand> Register() {
    yield return new Terminal.ConsoleCommand(
        "clearcolor",
        "(ColorfulPieces) Clears all colors applied to all pieces within radius of player.",
        RunLegacy);

    yield return new Terminal.ConsoleCommand(
        "clear-color",
        "(ColorfulPieces) clear-color --radius=<r> [--prefab=<name1>] [--position=<x,y,z>]",
        Run);
  }

  public static object RunLegacy(Terminal.ConsoleEventArgs args) {
    if (args.Length < 2 || !float.TryParse(args.Args[1], out float radius) || !Player.m_localPlayer) {
      return false;
    }

    Game.instance.StartCoroutine(
        ColorfulUtils.ClearColorsInRadiusCoroutine(Player.m_localPlayer.transform.position, radius, []));

    return true;
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    ColorfulPieces.LogInfo(args.Args.FullLine);

    if (!Player.m_localPlayer) {
      ColorfulPieces.LogError($"Missing local player.");
      return false;
    }

    if (!args.TryGetValue("radius", "r", out float radius)) {
      ColorfulPieces.LogError($"Missing --radius arg.");
      return false;
    }

    if (radius < 0f) {
      ColorfulPieces.LogError($"Invalid --radius arg, cannot be less than 0.");
      return false;
    }

    HashSet<int> prefabHashCodes = [];

    if (args.TryGetListValue("prefab", "p", out List<string> prefabs)) {
      foreach (string prefab in prefabs) {
        prefabHashCodes.Add(prefab.GetStableHashCode());
      }
    }

    if (!args.TryGetValue("position", "pos", out Vector3 position)) {
      position = Player.m_localPlayer.transform.position;
    }

    Game.instance.StartCoroutine(ColorfulUtils.ClearColorsInRadiusCoroutine(position, radius, prefabHashCodes));

    return true;
  }
}
