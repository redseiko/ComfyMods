using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PutMeDown.PluginConfig;

namespace PutMeDown {
  [HarmonyPatch(typeof(Player))]
  static class PlayerPatch {
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Player.AutoPickup))]
    static IEnumerable<CodeInstruction> AutoPickupTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldloc_S),
              new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ItemDrop), nameof(ItemDrop.m_autoPickup))))
          .ThrowIfInvalid("Could not transpiler patch Player.AutoPickup()!")
          .CopyInstruction(out CodeInstruction loadItemDropInstruction)
          .Advance(offset: 2)
          .InsertAndAdvance(
              loadItemDropInstruction,
              Transpilers.EmitDelegate<Func<bool, ItemDrop, bool>>(AutoPickupDelegate))
          .InstructionEnumeration();
    }

    static CodeMatcher CopyInstruction(this CodeMatcher matcher, out CodeInstruction instruction) {
      instruction = matcher.Instruction;
      return matcher;
    }

    static bool AutoPickupDelegate(bool autoPickup, ItemDrop itemDrop) {
      if (autoPickup && IsModEnabled.Value && AutoPickupController.ShouldIgnoreItem(itemDrop)) {
        return false;
      }

      return autoPickup;
    }
  }
}
