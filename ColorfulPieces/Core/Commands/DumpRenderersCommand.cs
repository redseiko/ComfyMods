namespace ColorfulPieces;

using System.IO;
using System.Text;

using ComfyLib;

using UnityEngine;

public static class DumpRenderersCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "dump-renderers",
        "(ColorfulPieces) dump-renderers --prefab=<prefab>",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    if (!args.TryGetValue("prefab", "p", out string prefabArg)) {
      return false;
    }

    if (!ZNetScene.s_instance.m_namedPrefabs.TryGetValue(prefabArg.GetStableHashCode(), out GameObject prefab)) {
      return false;
    }

    DumpPrefabRenderers(prefab);

    return true;
  }

  public static void DumpPrefabRenderers(GameObject prefab) {
    string filePath = $"{prefab.name.Replace("(Clone)", string.Empty)}_hierarchy.txt";

    if (File.Exists(filePath)) {
      return;
    }

    StringBuilder output = new StringBuilder();
    output.AppendLine($"--- Hierarchy for {prefab.name} ---");

    DumpRecursive(prefab.transform, output, "");

    File.WriteAllText(filePath, output.ToString());
    ColorfulPieces.LogInfo($"Dumped hierarchy to: {filePath}");
  }

  static void DumpRecursive(Transform transform, StringBuilder output, string indent) {
    if (transform.TryGetComponent(out Renderer renderer)) {
      string renderInfo = $" [HAS RENDERER: {renderer.GetType().Name}] (Mat: {renderer.sharedMaterial.Ref()?.name})";
      output.AppendLine($"{indent}- {transform.name}{renderInfo}");
    } else {
      output.AppendLine($"{indent}- {transform.name}");
    }

    for (int i = 0; i < transform.childCount; i++) {
      DumpRecursive(transform.GetChild(i), output, indent + "  ");
    }
  }
}
