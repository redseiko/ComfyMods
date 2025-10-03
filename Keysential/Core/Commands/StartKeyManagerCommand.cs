namespace Keysential;

using System.Collections;
using System.Collections.Generic;

using ComfyLib;

using UnityEngine;

public static class StartKeyManagerCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "start-key-manager",
        "(Keysential) start-key-manager --id=<manager-id> --position=<x,y,z> --distance=<8> --keys=<key1,key2> "
            + "[--type=<Vendor|DistanceXZ>] [--add]",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {    if (!args.TryGetValue("id", out string managerId)) {
      Keysential.LogError($"Missing string value for: --id");
      return false;
    }


    if (!args.TryGetValue("position", out Vector3 position)) {
      Keysential.LogError($"Missing or invalid Vector3 value for: --position");
      return false;
    }

    if (!args.TryGetValue("distance", out float distance) || distance < 0f) {
      Keysential.LogError($"Missing or invalid float value for: --distance");
      return false;
    }

    if (!args.TryGetListValue("keys", out List<string> keys) || keys.Count <= 0) {
      Keysential.LogError($"Missing or invalid string-list values for: --keys");
      return false;
    }

    if (!args.TryGetValue("type", out GlobalKeysManager.ManagerType managerType)) {
      managerType = GlobalKeysManager.ManagerType.DistanceXZ;
    }

    if (!args.TryGetValue("add", out bool addToStartUp)) {
      addToStartUp = false;
    }

    IEnumerator keyManagerCoroutine =
        GlobalKeysManager.CreateKeyManagerCoroutine(managerId, managerType, position, distance, keys);

    bool result = GlobalKeysManager.StartKeyManager(managerId, position, distance, keyManagerCoroutine);

    if (result && addToStartUp) {
      KeyManagerUtils.AddStartUpKeyManager(args.Args.FullLine);
    }

    return result;
  }
}
