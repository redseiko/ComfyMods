namespace ExternalConsole;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Console))]
static class ConsolePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Console.Awake))]
  static void AwakePostfix(Console __instance) {
    if (IsModEnabled.Value) {
      ConsoleManager.SetupExternalInput(__instance);
    }
  }
}
