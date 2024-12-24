namespace ColorfulPalettes;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
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
    if (takeInput && IsModEnabled.Value && ToggleConfigSelectShortcut.Value.IsDown()) {
      ConfigSelectController.Instance.ShowConfigSelect();
    }
  }
}
