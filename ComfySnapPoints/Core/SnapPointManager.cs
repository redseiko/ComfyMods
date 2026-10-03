namespace ComfySnapPoints;

public static class SnapPointManager {
  
  public static Piece LastRayPiece { get; private set; }

  public static void SetLastRayPiece(Piece piece) {
    LastRayPiece = piece;
  }

  public static int DestinationSnapIndex { get; set; } = -1;
}
