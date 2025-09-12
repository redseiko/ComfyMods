namespace Shortcuts;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Player.Update))]
  static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchGetKeyDown(0x7A)
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(ToggleDebugFlyDelegate))))
        .MatchGetKeyDown(0x62)
        .SetInstructionAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(ToggleDebugNoCostDelegate))))
        .MatchGetKeyDown(0x6B)
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(DebugKillAllDelegate))))
        .MatchGetKeyDown(0x6C)
        .SetInstructionAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(DebugRemoveDropsDelegate))))
        .MatchGetButtonDown("Hotbar1")
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(HotbarItem1Delegate))))
        .MatchGetButtonDown("Hotbar2")
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(HotbarItem2Delegate))))
        .MatchGetButtonDown("Hotbar3")
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(HotbarItem3Delegate))))
        .MatchGetButtonDown("Hotbar4")
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(HotbarItem4Delegate))))
        .MatchGetButtonDown("Hotbar5")
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(HotbarItem5Delegate))))
        .MatchGetButtonDown("Hotbar6")
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(HotbarItem6Delegate))))
        .MatchGetButtonDown("Hotbar7")
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(HotbarItem7Delegate))))
        .MatchGetButtonDown("Hotbar8")
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(HotbarItem8Delegate))))
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

  static bool HotbarItem1Delegate(string name) {
    return HotbarItem1Shortcut.IsKeyDown();
  }

  static bool HotbarItem2Delegate(string name) {
    return HotbarItem2Shortcut.IsKeyDown();
  }

  static bool HotbarItem3Delegate(string name) {
    return HotbarItem3Shortcut.IsKeyDown();
  }

  static bool HotbarItem4Delegate(string name) {
    return HotbarItem4Shortcut.IsKeyDown();
  }

  static bool HotbarItem5Delegate(string name) {
    return HotbarItem5Shortcut.IsKeyDown();
  }

  static bool HotbarItem6Delegate(string name) {
    return HotbarItem6Shortcut.IsKeyDown();
  }

  static bool HotbarItem7Delegate(string name) {
    return HotbarItem7Shortcut.IsKeyDown();
  }

  static bool HotbarItem8Delegate(string name) {
    return HotbarItem8Shortcut.IsKeyDown();
  }
}
