namespace PutMeDown;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Player.AutoPickup))]
  static IEnumerable<CodeInstruction> AutoPickupTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ItemDrop), nameof(ItemDrop.m_autoPickup))))
        .ThrowIfInvalid("Could not patch Player.AutoPickup()! (auto-pickup)")
        .CopyInstruction(out CodeInstruction loadItemDropInstruction)
        .Advance(offset: 2)
        .InsertAndAdvance(
            loadItemDropInstruction,
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(AutoPickupDelegate))))
        .InstructionEnumeration();
  }

  static CodeMatcher CopyInstruction(this CodeMatcher matcher, out CodeInstruction instruction) {
    instruction = new CodeInstruction(matcher.Opcode, matcher.Operand);
    return matcher;
  }

  static bool AutoPickupDelegate(bool autoPickup, ItemDrop itemDrop) {
    if (autoPickup && IsModEnabled.Value && AutoPickupController.ShouldIgnoreItem(itemDrop)) {
      return false;
    }

    return autoPickup;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Player.Update))]
  static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Player), nameof(Player.UpdateHover))))
        .ThrowIfInvalid($"Could not patch Player.Update()! (update-hover)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(UpdateHoverPostDelegate))))
        .InstructionEnumeration();
  }

  static void UpdateHoverPostDelegate(bool takeInput) {
    if (takeInput && IsModEnabled.Value && ItemsToIgnoreCycleShortcut.Value.IsDown()) {
      CycleItemsToIgnoreConfig();
    }
  }
}
