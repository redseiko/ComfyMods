using System.Collections;
using System.IO;
using System.Threading.Tasks;

using HarmonyLib;

namespace ExternalConsole {
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
        Register();
      }
    }

    static void Register() {
      new Terminal.ConsoleCommand(
          "sendpipe",
          "sendpipe",
          args => SendPipeCommand(args));
    }

    static bool SendPipeCommand(Terminal.ConsoleEventArgs args) {
      if (ExternalNamedPipe.PipeClient == null) {
        args.Context.AddString($"PipeClient is not initialized.");
        return false;
      }

      string[] parts = args.FullLine.Split(new char[] { ' ' }, 2, System.StringSplitOptions.RemoveEmptyEntries);

      if (parts.Length < 2) {
        args.Context.AddString($"Need to specify output to send.");
        return false;
      }

      return ExternalNamedPipe.SendPipeOutput(parts[1]);
    }
  }
}
