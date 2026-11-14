namespace BetterZeeLog;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ItemStand))]
static class ItemStandPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ItemStand.SetVisualItem))]
  static IEnumerable<CodeInstruction> SetVisualItemTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    if (!RemoveItemStandSetVisualItemLogging.Value) {
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
        .ThrowIfNotMatch("Could not patch ItemStand.SetVisualItem()! (log-missing-item-prefab)")
        .CreateLabelOffset(offset: 5, out Label retLabel)
        .ExtractLabels(out List<Label> ldstrLabels)
        .Insert(new CodeInstruction(OpCodes.Br, retLabel))
        .AddLabels(ldstrLabels)
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "Failed to get attach prefab for item "),
            new CodeMatch(OpCodes.Ldarga_S),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(int), nameof(int.ToString), [])),
            new CodeMatch(
                OpCodes.Call,
                AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZLog), nameof(ZLog.LogWarning))),
            new CodeMatch(OpCodes.Ret))
        .ThrowIfNotMatch("Could not patch ItemStand.SetVisualItem()! (log-failed-attach-prefab)")
        .CreateLabelOffset(offset: 5, out retLabel)
        .ExtractLabels(out ldstrLabels)
        .Insert(new CodeInstruction(OpCodes.Br, retLabel))
        .AddLabels(ldstrLabels)
        .InstructionEnumeration();
  }
}
