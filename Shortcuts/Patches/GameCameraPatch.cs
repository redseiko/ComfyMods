namespace Shortcuts;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(GameCamera))]
static class GameCameraPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(GameCamera.LateUpdate))]
  static IEnumerable<CodeInstruction> LateUpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchGetKeyDown(0x124)
        .SetInstructionAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(GameCameraPatch), nameof(TakeScreenshotDelegate))))
        .InstructionEnumeration();
  }

  static bool TakeScreenshotDelegate(KeyCode key, bool logWarning) {
    return TakeScreenshotShortcut.IsKeyDown();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(GameCamera.UpdateMouseCapture))]
  static IEnumerable<CodeInstruction> UpdateMouseCaptureTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchGetKey(0x132)
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameCameraPatch), nameof(IgnoreKeyDelegate))))
        .MatchGetKeyDown(0x11A)
        .SetInstructionAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(GameCameraPatch), nameof(ToggleMouseCaptureDelegate))))
        .InstructionEnumeration();
  }

  static bool ToggleMouseCaptureDelegate(KeyCode key, bool logWarning) {
    return ToggleMouseCaptureShortcut.IsKeyDown();
  }

  static bool IgnoreKeyDelegate(KeyCode key, bool logWarning) {
    return true;
  }
}
