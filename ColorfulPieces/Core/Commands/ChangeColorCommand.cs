namespace ColorfulPieces;

using System;
using System.Collections.Generic;

using ComfyLib;

public static class ChangeColorCommand {
  [ComfyCommand]
  public static IEnumerable<Terminal.ConsoleCommand> Register() {
    yield return new Terminal.ConsoleCommand(
        "changecolor",
        "(ColorfulPieces) Changes the color of all pieces within radius of player to the currently set color.",
        RunLegacy);

    yield return new Terminal.ConsoleCommand(
        "change-color",
        "(ColorfulPieces) change-color --radius=<r> [--prefab=<name1>]",
        Run);
  }

  public static object RunLegacy(Terminal.ConsoleEventArgs args) {
    if (args.Length < 2 || !float.TryParse(args.Args[1], out float radius) || !Player.m_localPlayer) {
      return false;
    }

    Game.instance.StartCoroutine(
        ColorfulPieces.ChangeColorsInRadiusCoroutine(
            Player.m_localPlayer.transform.position, radius, Array.Empty<int>()));

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

    HashSet<int> prefabHashCodes = new();

    if (args.TryGetListValue("prefab", "p", out List<string> prefabs)) {
      foreach (string prefab in prefabs) {
        prefabHashCodes.Add(prefab.GetStableHashCode());
      }
    }

    Game.instance.StartCoroutine(
        ColorfulPieces.ChangeColorsInRadiusCoroutine(
            Player.m_localPlayer.transform.position, radius, prefabHashCodes));

    return true;
  }
}
