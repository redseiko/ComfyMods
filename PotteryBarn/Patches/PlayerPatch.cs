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
        .SetInstructionAndAdvance(
            new CodeInstruction(
                OpCodes.Call,
                AccessTools.Method(typeof(PlayerPatch), nameof(SetupPlacementGhostInstantiateDelegate))))
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
  [HarmonyPatch(nameof(Player.SetupPlacementGhost))]
  static void SetupPlacementGhostPrefix() {
    PotteryManager.IsPlacingPiece = true;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.SetupPlacementGhost))]
  static void SetupPlacementGhostPostfix() {
    PotteryManager.IsPlacingPiece = false;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.CheckCanRemovePiece))]
  static void CheckCanRemovePostfix(Piece piece, ref bool __result) {
    if (!__result) {
      return;
    }

    if (PotteryManager.IsShopPiece(piece)) {
      __result = piece.IsCreator();
    } else if (PotteryManager.IsVanillaPiece(piece)) {
      __result = piece.IsPlacedByPlayer();
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Player.PlacePiece))]
  static IEnumerable<CodeInstruction> PlacePieceTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(WearNTear), nameof(WearNTear.OnPlaced))),
            new CodeMatch(OpCodes.Ldarg_S),
            new CodeMatch(OpCodes.Brfalse))
        .ThrowIfInvalid($"Could not patch Player.PlacePiece()! (on-placed)")
        .Advance(offset: 1)
        .ExtractLabels(out List<Label> doAttackLabels)
        .Insert(
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(
                OpCodes.Call,
                AccessTools.Method(typeof(PotteryManager), nameof(PotteryManager.PlacePieceDoAttackPreDelegate))))
        .AddLabels(doAttackLabels)
        .InstructionEnumeration();
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Player.PlacePiece))]
  static void PlacePiecePrefix() {
    PotteryManager.IsPlacingPiece = true;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.PlacePiece))]
  static void PlacePiecePostfix() {
    PotteryManager.IsPlacingPiece = false;
  }
}
