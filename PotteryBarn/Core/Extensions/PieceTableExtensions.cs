namespace PotteryBarn;

public static class PieceTableExtensions {
  public static PieceTable AddPiece(this PieceTable pieceTable, Piece piece) {
    if (!pieceTable.m_pieces.Contains(piece.gameObject)) {
      pieceTable.m_pieces.Add(piece.gameObject);
    }

    return pieceTable;
  }
}
