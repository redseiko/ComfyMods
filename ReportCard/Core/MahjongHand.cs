namespace ReportCard;

using System.Collections.Generic;

public sealed class MahjongHand {
  public List<MahjongTileInfo> HandTiles { get; } = [];
  public List<MahjongTileInfo> IncomingTiles { get; } = [];

  public void AddToHand(MahjongTileInfo tile) {
    HandTiles.Add(tile);
  }

  public void RemoveFromHand(MahjongTileInfo tile) {
    HandTiles.Remove(tile);
  }

  public void AddToIncoming(MahjongTileInfo tile) {
    IncomingTiles.Add(tile);
  }

  public void RemoveFromIncoming(MahjongTileInfo tile) {
    IncomingTiles.Remove(tile);
  }

  public void Clear() {
    HandTiles.Clear();
    IncomingTiles.Clear();
  }

  public void SortHand() {
    HandTiles.Sort((a, b) => {
      if (a.Suit != b.Suit) {
        return a.Suit.CompareTo(b.Suit);
      }
      return a.Rank.CompareTo(b.Rank);
    });
  }
}
