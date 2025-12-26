namespace ReportCard;

using System;

public static class MahjongTileHelper {
  static readonly Random Random = new();

  public static string GetFormattedTileText(MahjongTileInfo info) {
    switch (info.Suit) {
      case MahjongSuit.Characters:
        return $"{info.Rank}\nM";

      case MahjongSuit.Bamboos:
        return $"{info.Rank}\nS";

      case MahjongSuit.Dots:
        return $"{info.Rank}\nP";

      case MahjongSuit.Winds:
        string wind = info.Rank switch {
          1 => "E",
          2 => "S",
          3 => "W",
          4 => "N",
          _ => "?",
        };

        return $"{wind}\nWind";

      case MahjongSuit.Dragons:
        string dragon = info.Rank switch {
          1 => "R",
          2 => "G",
          3 => "W",
          _ => "?",
        };

        return $"{dragon}\nDrg";

      default:
        throw new ArgumentOutOfRangeException(nameof(info.Suit));
    }
  }

  public static MahjongTileInfo GetRandomTileInfo() {
    MahjongSuit randomSuit = (MahjongSuit) Random.Next(Enum.GetValues(typeof(MahjongSuit)).Length);

    int rank = randomSuit switch {
      MahjongSuit.Winds => Random.Next(1, 5),
      MahjongSuit.Dragons => Random.Next(1, 4),
      _ => Random.Next(1, 10),
    };

    return new MahjongTileInfo(randomSuit, rank);
  }

  static int _debugSuitIndex;
  static int _debugRankIndex = 1;

  public static MahjongTileInfo GetSequentialTileInfo() {
    MahjongSuit[] suits = (MahjongSuit[]) Enum.GetValues(typeof(MahjongSuit));
    MahjongSuit currentSuit = suits[_debugSuitIndex];

    int maxRank = currentSuit switch {
      MahjongSuit.Winds => 4,
      MahjongSuit.Dragons => 3,
      _ => 9
    };

    MahjongTileInfo info = new(currentSuit, _debugRankIndex);

    _debugRankIndex++;

    if (_debugRankIndex > maxRank) {
      _debugRankIndex = 1;
      _debugSuitIndex++;

      if (_debugSuitIndex >= suits.Length) {
        _debugSuitIndex = 0;
      }
    }

    return info;
  }
}
