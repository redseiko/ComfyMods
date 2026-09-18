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
  static IEnumerable<CodeInstruction> UpdateTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
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
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldc_I4_8),
            new CodeMatch(OpCodes.Ble))
        .ThrowIfInvalid($"Could not patch Player.Update()! (for-loop-8)")
        .Advance(offset: 1)
        .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_0))
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(HotbarItemsDelegate))))
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

  static void HotbarItemsDelegate(Player player) {
    if (HotbarItem1Shortcut.IsKeyDown()) {
      player.UseHotbarItem(1);
    }

    if (HotbarItem2Shortcut.IsKeyDown()) {
      player.UseHotbarItem(2);
    }

    if (HotbarItem3Shortcut.IsKeyDown()) {
      player.UseHotbarItem(3);
    }

    if (HotbarItem4Shortcut.IsKeyDown()) {
      player.UseHotbarItem(4);
    }

    if (HotbarItem5Shortcut.IsKeyDown()) {
      player.UseHotbarItem(5);
    }

    if (HotbarItem6Shortcut.IsKeyDown()) {
      player.UseHotbarItem(6);
    }

    if (HotbarItem6Shortcut.IsKeyDown()) {
      player.UseHotbarItem(6);
    }

    if (HotbarItem7Shortcut.IsKeyDown()) {
      player.UseHotbarItem(7);
    }

    if (HotbarItem8Shortcut.IsKeyDown()) {
      player.UseHotbarItem(8);
    }
  }
}
