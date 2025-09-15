namespace SearsCatalog;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Player.UpdateBuildGuiInput))]
  static IEnumerable<CodeInstruction> UpdateBuildGuiInputTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZInput), nameof(ZInput.GetMouseScrollWheel))))
        .ThrowIfInvalid("Could not patch Player.UpdateBuildGuiInput()! (get-mouse-scroll-wheel)")
        .Repeat(InsertGetMouseScrollWheelDelegate)
        .InstructionEnumeration();
  }

  static void InsertGetMouseScrollWheelDelegate(CodeMatcher matcher) {
    matcher
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayerPatch), nameof(GetAxisDelegate))));
  }

  static float GetAxisDelegate(float result) {
    return IsModEnabled.Value ? 0f : result;
  }
}
