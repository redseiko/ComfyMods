namespace ReportCard;

public enum MahjongSuit {
  Characters,
  Bamboos,
  Dots,
  Winds,
  Dragons,
}

public readonly struct MahjongTileInfo {
  public MahjongSuit Suit { get; }
  public int Rank { get; }

  public MahjongTileInfo(MahjongSuit suit, int rank) => (Suit, Rank) = (suit, rank);
}
