namespace PotteryBarn;

using System.Collections;
using System.Collections.Generic;

public sealed class PotteryPieceList : IEnumerable<PotteryPiece> {
  public readonly List<PotteryPiece> Pieces = [];
  public readonly HashSet<string> Prefabs = [];

  public PotteryPieceList Add(string piecePrefab, string craftingStation, ICollection<PieceResource> pieceResources) {
    Pieces.Add(new(piecePrefab, craftingStation, pieceResources));
    Prefabs.Add(piecePrefab);
    return this;
  }

  public bool Contains(string prefabName) => Prefabs.Contains(prefabName);

  public IEnumerator<PotteryPiece> GetEnumerator() {
    return Pieces.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() {
    return GetEnumerator();
  }
}

public sealed record class PotteryPiece(
    string PiecePrefab, string CraftingStation, ICollection<PieceResource> PieceResources);

public sealed class PieceResourceList : List<PieceResource> {
  public PieceResourceList Add(string resourceName, int resourceAmount) {
    Add(new(resourceName, resourceAmount));
    return this;
  }
}

public sealed record class PieceResource(string Name, int Amount);
