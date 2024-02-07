namespace ColorfulPieces;

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
  }
}
