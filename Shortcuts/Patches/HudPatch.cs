using System;
using System.Collections.Generic;

using HarmonyLib;

using UnityEngine;

using static Shortcuts.PluginConfig;

namespace Shortcuts {
  [HarmonyPatch(typeof(Hud))]
  static class HudPatch {
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Hud.Update))]
    static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchGetKeyDown(0x11C)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(ToggleHudDelegate))
          .MatchGetKey(0x132)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(IgnoreKeyDelegate))
          .InstructionEnumeration();
    }

    static bool ToggleHudDelegate(KeyCode key, bool logWarning) {
      return ToggleHudShortcut.IsKeyDown();
    }

    static bool IgnoreKeyDelegate(KeyCode key, bool logWarning) {
      return true;
    }
  }
}
