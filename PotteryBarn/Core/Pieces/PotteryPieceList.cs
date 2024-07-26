namespace PotteryBarn;

using System;
using System.Collections;
using System.Collections.Generic;

public sealed class PotteryPieceList : IEnumerable<PotteryPiece> {
  public readonly Dictionary<string, PotteryPiece> Pieces = [];

  public PotteryPieceList AddPiece(
      string piecePrefab, string craftingStation, ICollection<PieceResource> pieceResources) {
    Pieces[piecePrefab] = new(piecePrefab, craftingStation, pieceResources);
    return this;
  }

  public bool Contains(string prefabName) => Pieces.ContainsKey(prefabName);

  public IEnumerator<PotteryPiece> GetEnumerator() {
    return Pieces.Values.GetEnumerator();
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

public sealed record class PieceResource(string Name, int Amount) {
  public static implicit operator PieceResource(ValueTuple<string, int> tuple) {
    return new(tuple.Item1, tuple.Item2);
  }
}
