﻿namespace PotteryBarn;

using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Player.SetupPlacementGhost))]
  static IEnumerable<CodeInstruction> SetupPlacementGhostTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(
                OpCodes.Call,
                ReflectionUtils.GetGenericMethod(
                        typeof(UnityEngine.Object),
                        nameof(UnityEngine.Object.Instantiate),
                        genericParameterCount: 1,
                        [typeof(Type)])
                    .MakeGenericMethod(typeof(GameObject))),
            new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(Player), nameof(Player.m_placementGhost))))
        .ThrowIfInvalid("Could not patch Player.SetupPlacementGhost()! (m_placementGhost)")
        .SetInstructionAndAdvance(Transpilers.EmitDelegate(SetupPlacementGhostInstantiateDelegate))
        .InstructionEnumeration();
  }

  static GameObject SetupPlacementGhostInstantiateDelegate(GameObject selectedPrefab) {
    bool setActive = false;

    if (IsModEnabled.Value && PlacementGhostUtils.HasActiveComponents(selectedPrefab)) {
      setActive = selectedPrefab.activeSelf;
      selectedPrefab.SetActive(false);
    }

    GameObject clonedPrefab = UnityEngine.Object.Instantiate(selectedPrefab);

    if (IsModEnabled.Value && setActive) {
      selectedPrefab.SetActive(true);

      PlacementGhostUtils.DestroyActiveComponents(clonedPrefab);
      clonedPrefab.SetActive(true);
    }

    return clonedPrefab;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.CheckCanRemovePiece))]
  static bool CheckCanRemovePostfix(Piece piece, ref bool __result) {
    if (__result && !PotteryManager.CanBeRemoved(piece)) {
      __result = false;
    }

    return true;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Player.PlacePiece))]
  static IEnumerable<CodeInstruction> PlacePieceTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Humanoid), nameof(Humanoid.GetRightItem))),
            new CodeMatch(OpCodes.Stloc_S))
        .ThrowIfInvalid($"Could not patch Player.PlacePiece()! (GetRightItem)")
        .Advance(offset: 3)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_3),
            Transpilers.EmitDelegate(PlacePieceInstantiateDelegate))
        .InstructionEnumeration();
  }

  static void PlacePieceInstantiateDelegate(GameObject clonedObject) {
    if (clonedObject.TryGetComponent(out Container container)) {
      container.GetInventory().RemoveAll();
    }
  }
}
