namespace ColorfulPieces;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ComfyLib;

using UnityEngine;

using static ColorfulConstants;
using static PluginConfig;

public static class ColorfulUtils {
  public static void ChangePieceColorAction(WearNTear wearNTear) {
    SetPieceColor(wearNTear, ColorToVector3(TargetPieceColor.Value), TargetPieceEmissionColorFactor.Value);
  }

  public static void ClearPieceColorAction(WearNTear wearNTear) {
    SetPieceColor(wearNTear, NoColorVector3, NoEmissionColorFactor);
  }

  public static void SetPieceColor(WearNTear wearNTear, Vector3 colorVector3, float emissionColorFactor) {
    if (!TryClaimOwnership(wearNTear.m_nview)) {
      return;
    }

    SetPieceColorZDO(wearNTear.m_nview.m_zdo, colorVector3, emissionColorFactor);

    if (wearNTear.TryGetComponent(out PieceColor pieceColor)) {
      pieceColor.UpdateColors();
    }

    wearNTear.m_piece.Ref()?.m_placeEffect?.Create(wearNTear.transform.position, wearNTear.transform.rotation);
  }

  public static void SetPieceColorZDO(ZDO zdo, Vector3 colorVector3, float emissionColorFactor) {
    zdo.Set(PieceColorHashCode, colorVector3);
    zdo.Set(PieceEmissionColorFactorHashCode, emissionColorFactor);
    zdo.Set(PieceLastColoredByHashCode, Player.m_localPlayer.GetPlayerID());
    zdo.Set(PieceLastColoredByHostHashCode, PrivilegeManager.GetNetworkUserId());
  }

  public static bool TryClaimOwnership(ZNetView netView) {
    if (!netView || !netView.IsValid()) {
      return false;
    }

    if (!PrivateArea.CheckAccess(netView.transform.position, flash: true)) {
      return false;
    }

    if (netView.gameObject.TryGetComponentInChildren(out Container container)
        && (container.m_inUse || netView.m_zdo.GetInt(ZDOVars.s_inUse, 0) == 1)) {
      ColorfulPieces.LogError($"Container in target is currently in use!");
      return false;
    }

    netView.ClaimOwnership();
    return true;
  }

  public static bool CopyPieceColorAction(ZNetView netView) {
    if (!netView
        || !netView.IsValid()
        || !netView.m_zdo.TryGetVector3(PieceColorHashCode, out Vector3 colorAsVector)) {
      return false;
    }

    Color color = Vector3ToColor(colorAsVector);
    TargetPieceColor.SetValue(color);

    if (netView.m_zdo.TryGetFloat(PieceEmissionColorFactorHashCode, out float factor)) {
      TargetPieceEmissionColorFactor.Value = factor;
    }

    MessageHud.m_instance.Ref()?.ShowMessage(
        MessageHud.MessageType.TopLeft,
        $"Copied piece color: #{ColorUtility.ToHtmlStringRGB(color)} (f: {TargetPieceEmissionColorFactor.Value})");

    return true;
  }

  static readonly List<Piece> _piecesCache = [];

  public static IEnumerator ChangeColorsInRadiusCoroutine(
      Vector3 position, float radius, IReadOnlyCollection<int> prefabHashCodes) {
    yield return null;

    _piecesCache.Clear();

    GetAllPiecesInRadius(Player.m_localPlayer.transform.position, radius, _piecesCache);
    _piecesCache.RemoveAll(piece => !piece || !piece.m_nview || !piece.m_nview.IsValid());

    if (prefabHashCodes.Count() > 0) {
      _piecesCache.RemoveAll(piece => !prefabHashCodes.Contains(piece.m_nview.m_zdo.m_prefab));
    }

    long changeColorCount = 0L;

    foreach (Piece piece in _piecesCache) {
      if (changeColorCount % 5 == 0) {
        yield return null;
      }

      if (piece && piece.TryGetComponent(out WearNTear wearNTear) && wearNTear) {
        ChangePieceColorAction(wearNTear);
        changeColorCount++;
      }
    }

    ColorfulPieces.LogInfo($"Changed color of {changeColorCount} pieces within {radius} meters.");
    _piecesCache.Clear();
  }

  public static IEnumerator ClearColorsInRadiusCoroutine(
      Vector3 position, float radius, IReadOnlyCollection<int> prefabHashCodes) {
    yield return null;

    _piecesCache.Clear();

    GetAllPiecesInRadius(Player.m_localPlayer.transform.position, radius, _piecesCache);
    _piecesCache.RemoveAll(piece => !piece || !piece.m_nview || !piece.m_nview.IsValid());

    if (prefabHashCodes.Count() > 0) {
      _piecesCache.RemoveAll(piece => !prefabHashCodes.Contains(piece.m_nview.m_zdo.m_prefab));
    }

    long clearColorCount = 0L;

    foreach (Piece piece in _piecesCache) {
      if (clearColorCount % 5 == 0) {
        yield return null;
      }

      if (piece && piece.TryGetComponent(out WearNTear wearNTear) && wearNTear) {
        ClearPieceColorAction(wearNTear);
        clearColorCount++;
      }
    }

    ColorfulPieces.LogInfo($"Cleared colors from {clearColorCount} pieces within {radius} meters.");
    _piecesCache.Clear();
  }

  public static void GetAllPiecesInRadius(Vector3 position, float radius, List<Piece> pieces) {
    foreach (Piece piece in Piece.s_allPieces) {
      if (piece.gameObject.layer == Piece.s_ghostLayer
          || Vector3.Distance(position, piece.transform.position) >= radius) {
        continue;
      }

      pieces.Add(piece);
    }
  }
}
