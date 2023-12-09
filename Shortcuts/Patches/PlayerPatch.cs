using System;
using System.Collections.Generic;

using HarmonyLib;

using UnityEngine;

using static Shortcuts.PluginConfig;

namespace Shortcuts {
  [HarmonyPatch(typeof(Player))]
  static class PlayerPatch {
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Player.Update))]
    static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchGetKeyDown(0x7A)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(ToggleDebugFlyDelegate))
          .MatchGetKeyDown(0x62)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(ToggleDebugNoCostDelegate))
          .MatchGetKeyDown(0x6B)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(DebugKillAllDelegate))
          .MatchGetKeyDown(0x6C)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(DebugRemoveDropsDelegate))
          .MatchGetKeyDown(0x31)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(HotbarItem1Delegate))
          .MatchGetKeyDown(0x32)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(HotbarItem2Delegate))
          .MatchGetKeyDown(0x33)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(HotbarItem3Delegate))
          .MatchGetKeyDown(0x34)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(HotbarItem4Delegate))
          .MatchGetKeyDown(0x35)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(HotbarItem5Delegate))
          .MatchGetKeyDown(0x36)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(HotbarItem6Delegate))
          .MatchGetKeyDown(0x37)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(HotbarItem7Delegate))
          .MatchGetKeyDown(0x38)
          .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<KeyCode, bool, bool>>(HotbarItem8Delegate))
          .InstructionEnumeration();
    }

    static bool ToggleDebugFlyDelegate(KeyCode key, bool logWarning) {
      return ToggleDebugFlyShortcut.IsKeyDown();
    }

    static bool ToggleDebugNoCostDelegate(KeyCode key, bool logWarning) {
      return ToggleDebugNoCostShortcut.IsKeyDown();
    }

    static bool DebugKillAllDelegate(KeyCode key, bool logWarning) {
      return DebugKillAllShortcut.IsKeyDown();
    }

    static bool DebugRemoveDropsDelegate(KeyCode key, bool logWarning) {
      return DebugRemoveDropsShortcut.IsKeyDown();
    }

    static bool HotbarItem1Delegate(KeyCode key, bool logWarning) {
      return HotbarItem1Shortcut.IsKeyDown();
    }

    static bool HotbarItem2Delegate(KeyCode key, bool logWarning) {
      return HotbarItem2Shortcut.IsKeyDown();
    }

    static bool HotbarItem3Delegate(KeyCode key, bool logWarning) {
      return HotbarItem3Shortcut.IsKeyDown();
    }

    static bool HotbarItem4Delegate(KeyCode key, bool logWarning) {
      return HotbarItem4Shortcut.IsKeyDown();
    }

    static bool HotbarItem5Delegate(KeyCode key, bool logWarning) {
      return HotbarItem5Shortcut.IsKeyDown();
    }

    static bool HotbarItem6Delegate(KeyCode key, bool logWarning) {
      return HotbarItem6Shortcut.IsKeyDown();
    }

    static bool HotbarItem7Delegate(KeyCode key, bool logWarning) {
      return HotbarItem7Shortcut.IsKeyDown();
    }

    static bool HotbarItem8Delegate(KeyCode key, bool logWarning) {
      return HotbarItem8Shortcut.IsKeyDown();
    }
  }
}
