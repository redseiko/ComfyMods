namespace ConstructionDerby;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static PluginConfig;

public static class DerbyManager {
  public static DerbyGame CurrentGame { get; private set; }

  public static bool HasCurrentGame() {
    return CurrentGame != null;
  }

  public static IEnumerator StartGameCoroutine() {
    yield return null;

    Player player = Player.m_localPlayer;

    if (!player) {
      yield break;
    }

    int gameSeed = Random.Range(int.MinValue, int.MaxValue);

    ZPackage package = new();
    package.Write(player.GetPlayerID());
    package.Write(player.GetPlayerName());
    package.Write(gameSeed);

    ConstructionDerby.LogInfo($"Sending StartDerbyGame RPC to everyone with seed: {gameSeed}...");

    ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "StartDerbyGame", [package]);
  }

  public static void RPC_StartDerbyGame(long senderId, ZPackage package) {
    if (!IsModEnabled.Value) {
      return;
    }

    long playerId = package.ReadLong();
    string playerName = package.ReadString();
    int gameSeed = package.ReadInt();

    ConstructionDerby.LogInfo(
        $"Received StartDerbyGame RPC ... "
            + $"playerId: {playerId}, playerName: {playerName}, senderId: {senderId}, gameSeed: {gameSeed}");

    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"{playerName} wants to start Tetris building!");

    if (CurrentGame == null) {
      CurrentGame = new(gameSeed, GetDerbyGamePieces(Player.m_localPlayer));
      MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "... now starting!");
    } else {
      MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "... but you are already in a game.");
    }
  }

  public static List<Piece> GetDerbyGamePieces(Player player) {
    if (!player) {
      return [];
    }

    List<PieceTable> pieceTables = [];
    player.m_inventory.GetAllPieceTables(pieceTables);

    List<Piece> pieces = [];

    foreach (PieceTable pieceTable in pieceTables) {
      foreach (GameObject pieceObj in pieceTable.m_pieces) {
        if (pieceObj.TryGetComponent(out Piece piece)
            && !piece.m_repairPiece
            && piece.m_category == Piece.PieceCategory.BuildingWorkbench) {
          pieces.Add(piece);
        }
      }
    }

    ConstructionDerby.LogInfo($"Using {pieces.Count} pieces for DerbyGame.");
    return pieces;
  }

  public static IEnumerator StopGameCoroutine() {
    yield return null;

    if (CurrentGame == null) {
      yield break;
    }

    ConstructionDerby.LogInfo($"Sending StopDerbyGame RPC to everyone ...");
    ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "StopDerbyGame", []);
  }

  public static void RPC_StopDerbyGame(long senderId) {
    ConstructionDerby.LogInfo($"Received StopDerbyGame RPC ... senderId: {senderId}");

    if (CurrentGame == null) {
      return;
    }

    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "... current Derby building game stopped.");
    CurrentGame = null;

    Player.m_localPlayer.HideHandItems();
  }
}
