namespace ColorfulPieces;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;
using static PluginConstants;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public sealed class ColorfulPieces : BaseUnityPlugin {
  public const string PluginGUID = "redseiko.valheim.colorfulpieces";
  public const string PluginName = "ColorfulPieces";
  public const string PluginVersion = "1.15.1";

  static ManualLogSource _logger;
  Harmony _harmony;

  void Awake() {
    _logger = Logger;
    BindConfig(Config);

    _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
  }

  void OnDestroy() {
    _harmony?.UnpatchSelf();
  }

  static bool ClaimOwnership(WearNTear wearNTear) {
    if (!wearNTear
        || !wearNTear.m_nview
        || !wearNTear.m_nview.IsValid()
        || !PrivateArea.CheckAccess(wearNTear.transform.position, flash: true)) {
      return false;
    }

    wearNTear.m_nview.ClaimOwnership();

    return true;
  }

  public static void ChangePieceColorAction(WearNTear wearNTear) {
    if (!ClaimOwnership(wearNTear)) {
      return;
    }

    ChangePieceColorZdo(wearNTear.m_nview);

    if (wearNTear.TryGetComponent(out PieceColor pieceColor)) {
      pieceColor.UpdateColors();
    }

    wearNTear.m_piece.Ref()?.m_placeEffect?.Create(wearNTear.transform.position, wearNTear.transform.rotation);
  }

  // TODO(redseiko@): this is for applying PieceColor to other components than Piece.
  public static void ChangePieceColorAction(PieceColor pieceColor) {
    if (!pieceColor.TryGetComponent(out ZNetView netView)
        || !netView
        || !netView.IsValid()
        || !PrivateArea.CheckAccess(pieceColor.transform.position, flash: true)) {
      return;
    }

    netView.ClaimOwnership();
    ChangePieceColorZdo(netView);

    pieceColor.UpdateColors();

    Instantiate(
        ZNetScene.s_instance.GetPrefab("vfx_boar_love"),
        pieceColor.transform.position,
        pieceColor.transform.rotation);
  }

  static void ChangePieceColorZdo(ZNetView netView) {
    SetPieceColorZdoValues(
        netView.m_zdo, ColorToVector3(TargetPieceColor.Value), TargetPieceEmissionColorFactor.Value);
  }

  static readonly List<Piece> _piecesCache = new();

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

      if (piece && piece.TryGetComponent(out WearNTear wearNTear)) {
        ChangePieceColorAction(wearNTear);
        changeColorCount++;
      }
    }

    LogInfo($"Changed color of {changeColorCount} pieces within {radius} meters.");
    _piecesCache.Clear();
  }

  public static void ClearPieceColorAction(WearNTear wearNTear) {
    if (!ClaimOwnership(wearNTear)) {
      return;
    }

    SetPieceColorZdoValues(wearNTear.m_nview.m_zdo, NoColorVector3, NoEmissionColorFactor);

    if (wearNTear.TryGetComponent(out PieceColor pieceColor)) {
      pieceColor.UpdateColors();
    }

    wearNTear.m_piece.Ref()?.m_placeEffect?.Create(wearNTear.transform.position, wearNTear.transform.rotation);
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

      if (piece && piece.TryGetComponent(out WearNTear wearNTear)) {
        ClearPieceColorAction(wearNTear);
        clearColorCount++;
      }
    }

    LogInfo($"Cleared colors from {clearColorCount} pieces within {radius} meters.");
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

  public static void SetPieceColorZdoValues(ZDO zdo, Vector3 colorVector3, float emissionColorFactor) {
    zdo.Set(PieceColorHashCode, colorVector3);
    zdo.Set(PieceEmissionColorFactorHashCode, emissionColorFactor);
    zdo.Set(PieceLastColoredByHashCode, Player.m_localPlayer.GetPlayerID());
    zdo.Set(PieceLastColoredByHostHashCode, PrivilegeManager.GetNetworkUserId());
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

  public static void LogInfo(object obj) {
    _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
    Chat.m_instance.Ref()?.AddString(obj.ToString());
  }

  public static void LogError(object obj) {
    _logger.LogError($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {obj}");
    Chat.m_instance.Ref()?.AddString(obj.ToString());
  }
}
