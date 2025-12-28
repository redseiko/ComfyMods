namespace ReportCard;

using System.Collections.Generic;
using System.Linq;

public sealed class MahjongWall {
  public List<MahjongTileInfo> Tiles { get; private set; } = [];

  public int WallCount => Tiles.Count;

  public void Initialize() {
    Tiles = MahjongTileHelper.CreateFullDeck();
    Shuffle();
  }

  public void Shuffle() {
    System.Random rng = new();
    Tiles = Tiles.OrderBy(x => rng.Next()).ToList();
  }

  public bool TryDrawTile(out MahjongTileInfo tile) {
    if (Tiles.Count == 0) {
      tile = default;
      return false;
    }

    tile = Tiles[0];
    Tiles.RemoveAt(0);
    return true;
  }
}
