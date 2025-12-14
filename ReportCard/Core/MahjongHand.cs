namespace ReportCard;

using System.Collections.Generic;
using System.Linq;

using ComfyLib;

public class MahjongHand {
  public List<MahjongTileInfo> Tiles { get; } = [];
  public MahjongTileInfo IncomingTile { get; private set; }

  public void Initialize() {
    Tiles.Clear();
    for (int i = 0; i < 13; i++) {
      Tiles.Add(MahjongTileHelper.GetRandomTileInfo());
    }
    Sort();
    Draw(MahjongTileHelper.GetRandomTileInfo());
  }

  public void Draw(MahjongTileInfo tile) {
    if (IncomingTile != null) {
      // Logic error: already have an incoming tile
      // For now, let's just overwrite or ignore, but strictly we should probably throw or handle cleanup
      // In this simple prototype, we assume the controller manages flow correctly
    }
    IncomingTile = tile;
  }

  public MahjongTileInfo Discard(MahjongTileInfo tileToDiscard) {
    // If discarding the incoming tile
    if (tileToDiscard == IncomingTile) {
      MahjongTileInfo discarded = IncomingTile;
      IncomingTile = null;
      return discarded;
    }

    // Checking reference equality implicitly via Remove
    // However, List.Remove uses Equals currently.
    // Since MahjongTileInfo is a class without override, it uses ReferenceEquals.
    
    if (Tiles.Contains(tileToDiscard)) {
      Tiles.Remove(tileToDiscard);
      Tiles.Add(IncomingTile);
      IncomingTile = null;
      Sort();
      return tileToDiscard;
    }

    // Should not happen if UI is consistent
    return null;
  }

  public void Sort() {
    Tiles.Sort((a, b) => {
      if (a.Suit != b.Suit) return a.Suit.CompareTo(b.Suit);
      return a.Rank.CompareTo(b.Rank);
    });
  }
}
