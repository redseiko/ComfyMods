namespace PotteryBarn;

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

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Player.CheckCanRemovePiece))]
  static bool CheckCanRemovePrefix(Piece piece, ref bool __result) {
    if (IsModEnabled.Value && PotteryManager.IsShopPiece(piece)) {
      // Prevent player from removing world-generated pieces via build hammer.
      if (!piece.IsPlacedByPlayer()) {
        __result = false;
        return false;
      }

      // Prevent player from removing pieces they did not place themselves.
      if (!piece.IsCreator()) {
        __result = false;
        return false;
      }
    }

    return true;
  }
}
