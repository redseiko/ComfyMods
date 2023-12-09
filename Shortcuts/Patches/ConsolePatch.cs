using System;
using System.Collections.Generic;

using HarmonyLib;

using UnityEngine;

using static Shortcuts.PluginConfig;

namespace Shortcuts {
  [HarmonyPatch(typeof(Console))]
  static class ConsolePatch {
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Console.Update))]
    static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchGetKeyDown(0x11E)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(ToggleConsoleDelgate))
          .InstructionEnumeration();
    }

    static bool ToggleConsoleDelgate(KeyCode key, bool logWarning) {
      return ToggleConsoleShortcut.IsKeyDown();
    }
  }
}
