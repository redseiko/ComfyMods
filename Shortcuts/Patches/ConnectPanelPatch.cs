﻿namespace Shortcuts;

using System.Collections.Generic;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(ConnectPanel))]
static class ConnectPanelPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ConnectPanel.Update))]
  static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchGetKeyDown(0x11B)
        .SetInstructionAndAdvance(Transpilers.EmitDelegate(ToggleConnectPanelDelegate))
        .InstructionEnumeration();
  }

  static bool ToggleConnectPanelDelegate(KeyCode key, bool logWarning) {
    return ToggleConnectPanelShortcut.IsKeyDown();
  }
}
