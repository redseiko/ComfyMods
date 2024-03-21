namespace Shortcuts;

using System.Collections.Generic;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(FejdStartup.LateUpdate))]
  static IEnumerable<CodeInstruction> LateUpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchGetKeyDown(0x124)
        .SetInstructionAndAdvance(Transpilers.EmitDelegate(TakeScreenshotDelegate))
        .InstructionEnumeration();
  }

  static bool TakeScreenshotDelegate(KeyCode key, bool logWarning) {
    return TakeScreenshotShortcut.IsKeyDown();
  }
}
