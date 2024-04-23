namespace GetOffMyLawn;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public static class PieceUtils {
  public static bool CanRemovePiece(Piece piece) {
    return piece.TryGetComponentInChildren(out Ship _) || piece.TryGetComponentInChildren(out Vagon _);
  }

  public static bool RepairPiece(this Player player, ItemDrop.ItemData toolItem, Piece piece) {
    if (!piece || !piece.m_nview || !piece.m_nview.IsValid() || piece.TryGetComponent(out Plant _)) {
      return false;
    }

    if (!player.CheckCanRemovePiece(piece) || !PrivateArea.CheckAccess(piece.transform.position, 0f, flash: true)) {
      return false;
    }

    RepairPiece(piece, TargetPieceHealth.Value, Time.time);

    player.FaceLookDirection();
    player.m_zanim.SetTrigger(toolItem.m_shared.m_attack.m_attackAnimation);
    piece.m_placeEffect?.Create(piece.transform.position, piece.transform.rotation);

    if (ShowTopLeftMessageOnPieceRepair.Value) {
      player.Message(MessageHud.MessageType.TopLeft, Localization.instance.Localize("$msg_repaired", piece.m_name));
    }

    return true;
  }

  public static void RepairPiece(Piece piece, float targetHealth, float repairTime) {
    piece.m_nview.ClaimOwnership();
    piece.m_nview.m_zdo.Set(ZDOVars.s_health, targetHealth);

    if (piece.TryGetComponent(out WearNTear wearNTear)) {
      wearNTear.m_lastRepair = repairTime;
      wearNTear.m_healthPercentage = Mathf.Clamp01(targetHealth / wearNTear.m_health);
      wearNTear.SetHealthVisual(wearNTear.m_healthPercentage, triggerEffects: true);
    }
  }

  static readonly List<Piece> _pieceCache = new();
  static int _pieceCount = 0;

  public static void RepairPiecesInRadius(Vector3 origin, float radius) {
    _pieceCache.Clear();
    _pieceCount = 0;

    GetAllPiecesInRadius(origin, radius, _pieceCache);

    float targetHealth = TargetPieceHealth.Value;
    float repairTime = Time.time;

    foreach (Piece piece in _pieceCache) {
      if (!piece || !piece.m_nview || !piece.m_nview.IsValid() || piece.TryGetComponent(out Plant _)) {
        continue;
      }

      RepairPiece(piece, targetHealth, repairTime);

      if (ShowRepairEffectOnWardActivation.Value) {
        piece.m_placeEffect?.Create(piece.transform.position, piece.transform.rotation);
      }

      _pieceCount++;
    }

    GetOffMyLawn.LogInfo($"Repaired {_pieceCount} pieces to health: {TargetPieceHealth.Value}");

    if (ShowTopLeftMessageOnPieceRepair.Value) {
      Player.m_localPlayer.Message(
          MessageHud.MessageType.TopLeft, $"Repaired {_pieceCount} pieces to health: {TargetPieceHealth.Value}");
    }

    _pieceCache.Clear();
  }

  static void GetAllPiecesInRadius(Vector3 origin, float radius, List<Piece> pieces) {
    foreach (Piece piece in Piece.s_allPieces) {
      if (piece.gameObject.layer == Piece.s_ghostLayer
          || Vector3.Distance(origin, piece.transform.position) >= radius) {
        continue;
      }

      pieces.Add(piece);
    }
  }
}
