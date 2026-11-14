namespace BetterZeeLog;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(PickableItem))]
static class PickableItemPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(PickableItem.SetupItem))]
  static IEnumerable<CodeInstruction> SetupItemTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    if (!RemovePickableItemSetupItemLogging.Value) {
      return instructions;
    }

    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldstr, "Failed to get attach prefab for item "),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PickableItem), nameof(PickableItem.m_itemPrefab))),
            new CodeMatch(
                OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(UnityEngine.Object), nameof(UnityEngine.Object.name))),
            new CodeMatch(
                OpCodes.Call,
                AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)])),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ZLog), nameof(ZLog.LogWarning))),
            new CodeMatch(OpCodes.Ret))
        .ThrowIfNotMatch("Could not patch PickableItem.SetupItem()! (log-failed-attach-prfab)")
        .CreateLabelOffset(offset: 6, out Label retLabel)
        .ExtractLabels(out List<Label> ldstrLabels)
        .Insert(new CodeInstruction(OpCodes.Br, retLabel))
        .AddLabels(ldstrLabels)
        .InstructionEnumeration();
  }
}
