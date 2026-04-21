namespace ConstructionDerby;

using System.Collections.Generic;

public sealed class DerbyGame {
  public int GameSeed { get; }
  public System.Random GameRandom { get; }
  public List<Piece> GamePieces { get; }
  public Piece CurrentPiece { get; private set; }

  public DerbyGame(int seed, List<Piece> pieces) {
    GameSeed = seed;
    GameRandom = new(seed);
    GamePieces = [.. pieces];

    SelectNextPiece();
  }

  public Piece SelectNextPiece() {
    CurrentPiece = GamePieces[GameRandom.Next(GamePieces.Count)];
    return CurrentPiece;
  }
}
