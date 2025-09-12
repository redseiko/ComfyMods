namespace Shortcuts;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Hud))]
static class HudPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Hud.Update))]
  static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchGetKeyDown(0x11C)
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HudPatch), nameof(ToggleHudDelegate))))
        .MatchGetKey(0x132)
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HudPatch), nameof(IgnoreKeyDelegate))))
        .InstructionEnumeration();
  }

  static bool ToggleHudDelegate(KeyCode key, bool logWarning) {
    return ToggleHudShortcut.IsKeyDown();
  }

  static bool IgnoreKeyDelegate(KeyCode key, bool logWarning) {
    return true;
  }
}
