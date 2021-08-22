using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GetOffMyLawn {
  [BepInPlugin(Package, ModName, Version)]
  public class GetOffMyLawn : BaseUnityPlugin {
    public const string Package = "redseiko.valheim.getoffmylawn";
    public const string Version = "0.2.0";
    public const string ModName = "Get Off My Lawn";

    private static ConfigEntry<bool> _isModEnabled;
    private static ConfigEntry<float> _pieceHealth;

    private static ConfigEntry<bool> _showTopLeftMessageOnPieceRepair;
    private static ConfigEntry<bool> _showRepairEffectOnWardActivation;

    private static ManualLogSource _logger;
    private Harmony _harmony;

    private void Awake() {
      _isModEnabled = Config.Bind("Global", "isModEnabled", true, "Globally enable or disable this mod.");

      _pieceHealth =
          Config.Bind(
              "PieceValue",
              "targetPieceHealth",
              100_000_000f,
              "Target value to set piece health to when creating and repairing.");

      _showTopLeftMessageOnPieceRepair =
          Config.Bind(
              "Indicators",
              "showTopLeftMessageOnPieceRepair",
              false,
              "Shows a message in the top-left message area on piece repair.");

      _showRepairEffectOnWardActivation =
          Config.Bind(
              "Indicators",
              "showRepairEffectOnWardActivation",
              false,
              "Shows the repair effect on affected pieces when activating a ward.");

      _logger = Logger;
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void OnDestroy() {
      if (_harmony != null) {
        _harmony.UnpatchSelf();
      }
    }

    [HarmonyPatch(typeof(BaseAI))]
    private class BaseAiPatch {
      private static readonly string[] _targetRayMask = new string[] {
        "Default", "static_solid", "Default_small", "vehicle",
      };

      [HarmonyPostfix]
      [HarmonyPatch(nameof(BaseAI.Awake))]
      private static void AwakePostfix(ref BaseAI __instance) {
        if (_isModEnabled.Value) {
          __instance.m_monsterTargetRayMask = LayerMask.GetMask(_targetRayMask);
        }
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(BaseAI.FindRandomStaticTarget))]
      private static bool FindRandomStaticTargetPrefix(ref StaticTarget __result) {
        if (!_isModEnabled.Value) {
          return true;
        }

        __result = null;
        return false;
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(BaseAI.FindClosestStaticPriorityTarget))]
      private static bool FindClosestStaticPriorityTargetPrefix(ref StaticTarget __result) {
        if (!_isModEnabled.Value) {
          return true;
        }

        __result = null;
        return false;
      }
    }

    [HarmonyPatch(typeof(MonsterAI))]
    private class MonsterAiPatch {
      [HarmonyPrefix]
      [HarmonyPatch(nameof(MonsterAI.UpdateTarget))]
      private static void UpdateTargetPrefix(ref MonsterAI __instance) {
        if (_isModEnabled.Value) {
          __instance.m_attackPlayerObjects = false;
        }
      }
    }

    [HarmonyPatch(typeof(Piece))]
    private class PiecePatch {
      private static readonly int _healthHashcode = "health".GetStableHashCode();

      [HarmonyPostfix]
      [HarmonyPatch(nameof(Piece.SetCreator))]
      private static void SetCreatorPostfix(ref Piece __instance) {
        if (!_isModEnabled.Value || !__instance || !__instance.m_nview || __instance.GetComponent<Plant>()) {
          return;
        }

        _logger.LogInfo(
            string.Format(
                "Creating piece '{0}' with health: {1}",
                Localization.instance.Localize(__instance.m_name), _pieceHealth.Value));

        __instance.m_nview.GetZDO().Set(_healthHashcode, _pieceHealth.Value);
      }
    }

    [HarmonyPatch(typeof(PrivateArea))]
    private class PrivateAreaPatch {
      private static readonly int _healthHashCode = "health".GetStableHashCode();
      private static readonly List<Piece> _pieces = new();
      private static int _pieceCount = 0;

      [HarmonyPostfix]
      [HarmonyPatch(nameof(PrivateArea.Interact))]
      private static void InteractPostfix(ref PrivateArea __instance) {
        if (!_isModEnabled.Value || !__instance || !__instance.IsEnabled() || !__instance.m_piece.IsCreator()) {
          return;
        }

        _pieces.Clear();
        _pieceCount = 0;

        Piece.GetAllPiecesInRadius(__instance.transform.position, __instance.m_radius, _pieces);

        foreach (Piece piece in _pieces) {
          if (piece.GetComponent<Plant>()) {
            continue;
          }

          piece.m_nview.GetZDO().Set(_healthHashCode, _pieceHealth.Value);

          if (_showRepairEffectOnWardActivation.Value) {
            piece.m_placeEffect.Create(piece.transform.position, piece.transform.rotation);
          }

          _pieceCount++;
        }

        _logger.LogInfo($"Repaired {_pieceCount} pieces to health: {_pieceHealth.Value}");

        if (_showTopLeftMessageOnPieceRepair.Value) {
          Player.m_localPlayer.Message(
              MessageHud.MessageType.TopLeft, $"Repaired {_pieceCount} pieces to health: {_pieceHealth.Value}");
        }
      }
    }

    [HarmonyPatch(typeof(Player))]
    private class PlayerPatch {
      private static readonly int _healthHashCode = "health".GetStableHashCode();

      [HarmonyPrefix]
      [HarmonyPatch(nameof(Player.Repair))]
      private static void RepairPrefix(ref Player __instance, ItemDrop.ItemData toolItem, Piece repairPiece) {
        if (!_isModEnabled.Value || !__instance || !__instance.InPlaceMode()) {
          return;
        }

        Piece hoveringPiece = __instance.m_hoveringPiece;

        if (!hoveringPiece || !__instance.CheckCanRemovePiece(hoveringPiece) || hoveringPiece.GetComponent<Plant>()) {
          return;
        }

        string pieceName = Localization.instance.Localize(hoveringPiece.m_name);

        if (!PrivateArea.CheckAccess(hoveringPiece.transform.position, flash: true)) {
          _logger.LogInfo($"Unable to repair piece '{pieceName}' due to ward in range.");
          return;
        }

        hoveringPiece.m_nview.GetZDO().Set(_healthHashCode, _pieceHealth.Value);
        _logger.LogInfo($"Repaired piece '{pieceName}' to health: {_pieceHealth.Value}");

        if (_showTopLeftMessageOnPieceRepair.Value) {
          __instance.Message(
              MessageHud.MessageType.TopLeft, $"Repaired piece '{pieceName}' to health: {_pieceHealth.Value}");
        }
      }
    }
  }
}
