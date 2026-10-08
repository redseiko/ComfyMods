namespace Perpetuity;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(PersistentEventSystem.PersistentEvent))]
static class PersistentEventPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(PersistentEventSystem.PersistentEvent.GenerateEventLocation))]
  static IEnumerable<CodeInstruction> GenerateEventLocationTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(
                OpCodes.Newobj,
                AccessTools.Constructor(typeof(Vector3), [typeof(float), typeof(float), typeof(float)])),
            new CodeMatch(OpCodes.Stobj, typeof(Vector3)))
        .ThrowIfInvalid($"Could not patch PersistentEvent.GenerateEventLocation()! (normalize-position)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(PersistentEventPatch), nameof(NormalizePositionDelegate))))
        .MatchStartForward(
            new CodeMatch(OpCodes.Endfinally),
            new CodeMatch(IsLdLocS8),
            new CodeMatch(OpCodes.Brtrue))
        .ThrowIfInvalid($"Could not patch PersistentEvent.GenerateEventLocation()! (check-nearby-players)")
        .Advance(offset: 1)
        .CopyOperand(out LocalBuilder local8)
        .ExtractLabels(out List<Label> ldloc8Labels)
        .Insert(
            new CodeInstruction(OpCodes.Ldloc_S, local8),
            new CodeInstruction(OpCodes.Ldarg_2),
            new CodeInstruction(OpCodes.Ldobj, typeof(Vector3)),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(PersistentEventPatch), nameof(CheckNearbyPlayersDelegate))),
            new CodeInstruction(OpCodes.Stloc_S, local8))
        .AddLabels(ldloc8Labels)
        .InstructionEnumeration();

    static bool IsLdLocS8(CodeInstruction instruction) {
      return
          instruction.opcode == OpCodes.Ldloc_S
          && instruction.operand is LocalBuilder local
          && local.LocalIndex == 8;
    }
  }

  public static Vector3 NormalizePositionDelegate(Vector3 position) {
    if (CenterPersistentEventPosition.Value) {
      return new Vector3(
          Utils.FloorToInt((position.x + 32f) / 64f) * 64f,
          0f,
          Utils.FloorToInt((position.z + 32f) / 64f) * 64f);
    }

    return position;
  }

  public static bool CheckNearbyPlayersDelegate(bool isPlayerNearby, Vector3 position) {
    if (!isPlayerNearby
        && LocationInstancesBlockPeristentEventPlacement.Value
        && ZoneSystem.s_instance.m_locationInstances.TryGetValue(ZoneSystem.GetZone(position), out _)) {
      return true;
    }

    return isPlayerNearby;
  }
}
