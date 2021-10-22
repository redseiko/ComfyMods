using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using HarmonyLib;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace ConstructionDerby {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class ConstructionDerby : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.comfymods.constructionderby";
    public const string PluginName = "ConstructionDerby";
    public const string PluginVersion = "1.0.0";

    static ConfigEntry<bool> _isModEnabled;
    static ConfigEntry<bool> _canRemovePiece;

    static ManualLogSource _logger;
    Harmony _harmony;

    public void Awake() {
      _logger = Logger;

      _isModEnabled = Config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");
      _canRemovePiece = Config.Bind("Testing", "canRemovePiece", false, "Control if Player can remove a placed piece.");

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    class DerbyGame {
      public int GameSeed { get; }
      public System.Random GameRandom { get; }
      public List<Piece> GamePieces { get; }
      public Piece CurrentPiece { get; private set; }

      public DerbyGame(int seed, List<Piece> pieces) {
        GameSeed = seed;
        GameRandom = new(seed);
        GamePieces = new(pieces);
        SelectNextPiece();
      }

      public Piece SelectNextPiece() {
        CurrentPiece = GamePieces[GameRandom.Next(GamePieces.Count)];
        return CurrentPiece;
      }
    }

    static DerbyGame _currentGame;

    [HarmonyPatch(typeof(Player))]
    class PlayerPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(Player.GetBuildPieces))]
      static void GetBuildPiecesPostfix(ref Player __instance, ref List<Piece> __result) {
        if (!_isModEnabled.Value || _currentGame == null) {
          return;
        }

        __result.Clear();
        __result.Add(_currentGame.CurrentPiece);
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(Player.PlacePiece))]
      static void PlacePiecePostfix(ref Player __instance, ref bool __result) {
        if (!_isModEnabled.Value || !__result | _currentGame == null) {
          return;
        }

        _currentGame.SelectNextPiece();
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(Player.CheckCanRemovePiece))]
      static void CheckCanRemovePiece(ref bool __result) {
        if (!_isModEnabled.Value) {
          return;
        }

        __result = _canRemovePiece.Value;
      }
    }

    [HarmonyPatch(typeof(PieceTable))]
    class PieceTablePatch {
      [HarmonyPrefix]
      [HarmonyPatch(nameof(PieceTable.UpdateAvailable))]
      static void UpdateAvailablePrefix(ref PieceTable __instance) {
        if (!_isModEnabled.Value) {
          return;
        }

        foreach (GameObject gameObj in __instance.m_pieces) {
          if (gameObj.TryGetComponent(out Piece piece) && !piece.m_enabled) {
            _logger.LogInfo($"Piece {piece.m_name} was disabled ... enabling.");
            piece.m_enabled = true;
          }
        }
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(PieceTable.GetSelectedPrefab))]
      static void GetSelectedPrefabPostfix(ref GameObject __result) {
        if (!_isModEnabled.Value || _currentGame == null) {
          return;
        }

        __result = _currentGame.CurrentPiece.gameObject;
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(PieceTable.GetSelectedPiece))]
      static void GetSelectedPiecePostfix(ref Piece __result) {
        if (!_isModEnabled.Value || _currentGame == null) {
          return;
        }

        __result = _currentGame.CurrentPiece;
      }
    }

    [HarmonyPatch(typeof(Chat))]
    class ChatPatch {
      [HarmonyPrefix]
      [HarmonyPatch(nameof(Chat.InputText))]
      static bool InputTextPrefix(ref Chat __instance) {
        if (_isModEnabled.Value && ParseText(__instance)) {
          __instance.AddString(__instance.m_input.text);
          return false;
        }

        return true;
      }
    }

    static bool ParseText(Terminal terminal) {
      if (terminal.m_input.text == "/startgame") {
        terminal.StartCoroutine(StartGameCoroutine());
        return true;
      }

      if (terminal.m_input.text == "/stopgame") {
        terminal.StartCoroutine(StopGameCoroutine());
        return true;
      }

      return false;
    }

    [HarmonyPatch(typeof(ZNet))]
    class ZNetPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(ZNet.Awake))]
      static void AwakePostfix(ref ZNet __instance) {
        if (!_isModEnabled.Value) {
          return;
        }

        __instance.m_routedRpc.Register("StartTetrisGame", new Action<long, ZPackage>(RPC_StartTetrisGame));
        __instance.m_routedRpc.Register("StopTetrisGame", new Action<long>(RPC_StopTetrisGame));
      }
    }

    static IEnumerator StartGameCoroutine() {
      yield return null;

      Player player = Player.m_localPlayer;

      if (!player) {
        yield break;
      }

      int gameSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

      ZPackage package = new();
      package.Write(player.GetPlayerID());
      package.Write(player.GetPlayerName());
      package.Write(gameSeed);

      _logger.LogInfo($"Sending StartTetrisGame RPC to everyone with seed: {gameSeed}...");

      ZRoutedRpc.instance?.InvokeRoutedRPC(ZRoutedRpc.Everybody, "StartTetrisGame", new object[] { package });
    }

    static void RPC_StartTetrisGame(long senderId, ZPackage package) {
      long playerId = package.ReadLong();
      string playerName = package.ReadString();
      int gameSeed = package.ReadInt();

      _logger.LogInfo(
          $"Received StartTetrisGame RPC ... "
              + $"playerId: {playerId}, playerName: {playerName}, senderId: {senderId}, gameSeed: {gameSeed}");

      MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"{playerName} wants to start Tetris building!");

      if (_currentGame == null) {
        _currentGame = new(gameSeed, GetDerbyGamePieces(Player.m_localPlayer));
        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "... now starting!");
      } else {
        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "... but you are already in a game.");
      }
    }

    // static readonly string _hammerPieceTableName = "_HammerPieceTable";

    static List<Piece> GetDerbyGamePieces(Player player) {
      if (!player) {
        return new();
      }

      List<PieceTable> pieceTables = new();
      player.m_inventory.GetAllPieceTables(pieceTables);

      List<Piece> pieces = new();

      foreach (PieceTable pieceTable in pieceTables) {
        foreach (GameObject pieceObj in pieceTable.m_pieces) {
          if (pieceObj.TryGetComponent(out Piece piece)
              && !piece.m_repairPiece
              && piece.m_category == Piece.PieceCategory.Building) {
            pieces.Add(piece);
          }
        }
      }

      _logger.LogInfo($"Using {pieces.Count} pieces for DerbyGame.");
      return pieces;
    }

    static IEnumerator StopGameCoroutine() {
      yield return null;

      if (_currentGame == null) {
        yield break;
      }

      _logger.LogInfo($"Sending StopTetrisGame RPC to everyone ...");
      ZRoutedRpc.instance?.InvokeRoutedRPC(ZRoutedRpc.Everybody, "StopTetrisGame", new object[] { });
    }

    static void RPC_StopTetrisGame(long senderId) {
      _logger.LogInfo($"Received StopTetrisGame RPC ... senderId: {senderId}");

      if (_currentGame == null) {
        return;
      }

      MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "... current Tetris building game stopped.");
      _currentGame = null;

      Player.m_localPlayer?.HideHandItems();
    }
  }
}
