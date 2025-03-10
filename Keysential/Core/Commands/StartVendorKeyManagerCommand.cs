namespace Keysential;

using ComfyLib;

using UnityEngine;

public static class StartVendorKeyManagerCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "startvendorkeymanager",
        "startvendorkeymanager <id: id1> <position: x,y,z> <distance: 8f> <keys: key1,key2>",
        args => Run(args));
  }

  public static bool Run(Terminal.ConsoleEventArgs args) {
    if (args.Length < 5) {
      Keysential.LogError($"Not enough args for startvendorkeymanager command.");
      return false;
    }

    string managerId = args[1];

    if (!args[2].TryParseVector3(out Vector3 position)) {
      Keysential.LogError($"Could not parse Vector3 position arg: {args[2]}");
      return false;
    }

    if (!float.TryParse(args[3], out float distance) || distance < 0f) {
      Keysential.LogError($"Could not parse or invalid float distance arg: {args[3]}");
      return false;
    }

    string[] keys = args[4].GetEncodedGlobalKeys();

    if (keys.Length <= 0) {
      Keysential.LogError($"No valid values for keys arg: {args[4]}");
      return false;
    }

    return GlobalKeysManager.StartKeyManager(
        managerId,
        position,
        distance,
        VendorKeyManager.VendorPlayerProximityCoroutine(managerId, position, distance, keys));
  }
}
