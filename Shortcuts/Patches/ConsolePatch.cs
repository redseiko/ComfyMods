namespace Shortcuts;

using System.Collections.Generic;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Console))]
static class ConsolePatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Console.Update))]
  static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchGetButtonDown("Console")
        .SetInstructionAndAdvance(Transpilers.EmitDelegate(ToggleConsoleDelgate))
        .InstructionEnumeration();
  }

  static bool ToggleConsoleDelgate(string name) {
    return ToggleConsoleShortcut.IsKeyDown();
  }
}
