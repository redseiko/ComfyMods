namespace BetterZeeLog;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ArmorStand))]
static class ArmorStandPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ArmorStand.SetVisualItem))]
  static IEnumerable<CodeInstruction> SetVisualItemTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    if (!RemoveArmorStandSetVisualItemLogging.Value) {
      return instructions;
    }

    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "Missing item prefab "),
            new CodeMatch(OpCodes.Ldarga_S),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(int), nameof(int.ToString), [])),
            new CodeMatch(
                OpCodes.Call,
                AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZLog), nameof(ZLog.LogWarning))),
            new CodeMatch(OpCodes.Ret))
        .ThrowIfNotMatch("Could not patch ArmorStand.SetVisualItem()! (log-missing-item-prefab)")
        .CreateLabelOffset(offset: 5, out Label retLabel)
        .ExtractLabels(out List<Label> ldstrLabel)
        .Insert(new CodeInstruction(OpCodes.Br, retLabel))
        .AddLabels(ldstrLabel)
        .InstructionEnumeration();
  }
}
