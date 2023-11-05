using System.IO;

using HarmonyLib;

using static ExternalConsole.PluginConfig;

namespace ExternalConsole {
  [HarmonyPatch(typeof(Console))]
  static class ConsolePatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Console.Awake))]
    static void AwakePostfix(ref Console __instance) {
      if (IsModEnabled.Value && ExternalInputFilename.Value.Length > 0) {
        __instance.StartCoroutine(
            ExternalInputFile.ReadFromFileCoroutine(
                Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), ExternalInputFilename.Value)));
      }
    }
  }
}
