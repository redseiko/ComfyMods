using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using HarmonyLib;

using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace GetOffMyLawn {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class GetOffMyLawn : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.getoffmylawn";
    public const string PluginName = "GetOffMyLawn";
    public const string PluginVersion = "1.0.1";

    static ConfigEntry<bool> _isModEnabled;
    static ConfigEntry<float> _pieceHealth;

    static ConfigEntry<bool> _showTopLeftMessageOnPieceRepair;
    static ConfigEntry<bool> _showRepairEffectOnWardActivation;

    static ManualLogSource _logger;
    Harmony _harmony;

    public void Awake() {
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
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    [HarmonyPatch(typeof(BaseAI))]
    class BaseAiPatch {
      static readonly string[] _targetRayMask = new string[] {
        "Default", "static_solid", "Default_small", "vehicle",
      };

      [HarmonyPostfix]
      [HarmonyPatch(nameof(BaseAI.Awake))]
      static void AwakePostfix(ref BaseAI __instance) {
        if (_isModEnabled.Value) {
          __instance.m_monsterTargetRayMask = LayerMask.GetMask(_targetRayMask);
        }
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(BaseAI.FindRandomStaticTarget))]
      static bool FindRandomStaticTargetPrefix(ref StaticTarget __result) {
        if (!_isModEnabled.Value) {
          return true;
        }

        __result = null;
        return false;
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(BaseAI.FindClosestStaticPriorityTarget))]
      static bool FindClosestStaticPriorityTargetPrefix(ref StaticTarget __result) {
        if (!_isModEnabled.Value) {
          return true;
        }

        __result = null;
        return false;
      }
    }

    [HarmonyPatch(typeof(MonsterAI))]
    class MonsterAiPatch {
      [HarmonyPrefix]
      [HarmonyPatch(nameof(MonsterAI.UpdateTarget))]
      static void UpdateTargetPrefix(ref MonsterAI __instance) {
        if (_isModEnabled.Value) {
          __instance.m_attackPlayerObjects = false;
        }
      }
    }

    [HarmonyPatch(typeof(Piece))]
    class PiecePatch {
      static readonly int _healthHashCode = "health".GetStableHashCode();

      [HarmonyPostfix]
      [HarmonyPatch(nameof(Piece.SetCreator))]
      static void SetCreatorPostfix(ref Piece __instance) {
        if (!_isModEnabled.Value || !__instance || !__instance.m_nview || __instance.GetComponent<Plant>()) {
          return;
        }

        _logger.LogInfo(
            $"Creating piece '{Localization.instance.Localize(__instance.m_name)}' with health: {_pieceHealth.Value}");

        __instance.m_nview.GetZDO().Set(_healthHashCode, _pieceHealth.Value);
      }
    }

    [HarmonyPatch(typeof(PrivateArea))]
    class PrivateAreaPatch {
      static readonly int _healthHashCode = "health".GetStableHashCode();
      static readonly List<Piece> _pieces = new();
      static int _pieceCount = 0;

      [HarmonyPostfix]
      [HarmonyPatch(nameof(PrivateArea.Interact))]
      static void InteractPostfix(ref PrivateArea __instance) {
        if (!_isModEnabled.Value || !__instance || !__instance.IsEnabled() || !__instance.m_piece.IsCreator()) {
          return;
        }

        _pieces.Clear();
        _pieceCount = 0;

        Piece.GetAllPiecesInRadius(__instance.transform.position, __instance.m_radius, _pieces);

        foreach (Piece piece in _pieces) {
          if (!piece || !piece.m_nview || !piece.m_nview.IsValid()) {
            _logger.LogWarning(
                $"Skipping piece with invalid ZNetView: {Localization.instance.Localize(piece?.m_name ?? "null")}.");

            continue;
          }

          if (piece.GetComponent<Plant>()) {
            continue;
          }

          piece.m_nview.GetZDO().Set(_healthHashCode, _pieceHealth.Value);

          if (_showRepairEffectOnWardActivation.Value) {
            piece.m_placeEffect?.Create(piece.transform.position, piece.transform.rotation);
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
    class PlayerPatch {
      static readonly int _healthHashCode = "health".GetStableHashCode();

      [HarmonyPrefix]
      [HarmonyPatch(nameof(Player.Repair))]
      static void RepairPrefix(ref Player __instance, ItemDrop.ItemData toolItem, Piece repairPiece) {
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

        if (!hoveringPiece.m_nview || !hoveringPiece.m_nview.IsValid()) {
          _logger.LogWarning($"Unable to repair piece '{pieceName}' due to invalid ZNetView.");
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
