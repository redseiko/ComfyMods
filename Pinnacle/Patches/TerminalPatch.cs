namespace Pinnacle;

using BepInEx.Logging;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Terminal))]
static class TerminalPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(Terminal.InitTerminal))]
  static void InitTerminalPrefix(ref bool __state) {
    __state = Terminal.m_terminalInitialized;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Terminal.InitTerminal))]
  static void InitTerminalPostfix(bool __state) {
    if (!__state) {
      ComfyCommandUtils.ToggleCommands(IsModEnabled.Value);
    }

    if (IsModEnabled.Value) {
      ModifyResetMapCommand();
    }
  }

  static void ModifyResetMapCommand() {
    if (Terminal.commands.TryGetValue("resetmap", out Terminal.ConsoleCommand command)) {
      command.IsCheat = false;
      command.OnlyServer = false;

      Pinnacle.Log(LogLevel.Info, $"Modified 'resetmap' command: IsCheat = false, OnlyServer = false.");
    }
  }
}
