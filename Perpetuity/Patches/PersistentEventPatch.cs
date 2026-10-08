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
  static IEnumerable<CodeInstruction> GenerateEventLocation1Transpiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(IsLdLocS6),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(
                OpCodes.Ldfld,
                AccessTools.Field(
                    typeof(PersistentEventSystem.PersistentEvent),
                    nameof(PersistentEventSystem.PersistentEvent.maxDistanceFromCenter))),
            new CodeMatch(
                OpCodes.Ldarg_0),
            new CodeMatch(
                OpCodes.Ldfld,
                AccessTools.Field(
                    typeof(PersistentEventSystem.PersistentEvent),
                    nameof(PersistentEventSystem.PersistentEvent.minDistanceFromCenter))),
            new CodeMatch(OpCodes.Sub))
        .ThrowIfInvalid($"Could not patch PersistentEvent.GenerateEventLocation()! (normalize-vector)")
        .CopyOperand(out LocalBuilder local6)
        .Advance(offset: 6)
        .MatchStartForward(
            new CodeMatch(OpCodes.Stloc_S, local6),
            new CodeMatch(OpCodes.Ldarg_2),
            new CodeMatch(OpCodes.Ldloc_S, local6))
        .ThrowIfInvalid($"Could not patch PersistentEvent.GenerateEventLocation()! (save-normalize-vector)")
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(PersistentEventPatch), nameof(NormalizePositionDelegate))))
        .InstructionEnumeration();

    static bool IsLdLocS6(CodeInstruction instruction) {
      return
          instruction.opcode == OpCodes.Ldloc_S
          && instruction.operand is LocalBuilder local
          && local.LocalIndex == 6;
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(PersistentEventSystem.PersistentEvent.GenerateEventLocation))]
  static IEnumerable<CodeInstruction> GenerateEventLocation2Transpiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
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

  public static Vector2 NormalizePositionDelegate(Vector2 position) {
    if (CenterPersistentEventPosition.Value) {
      return new Vector2(
          Utils.FloorToInt((position.x + 32f) / 64f) * 64f,
          Utils.FloorToInt((position.y + 32f) / 64f) * 64f);
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
