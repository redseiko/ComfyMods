namespace PotteryBarn;

using static PieceConstants;

public static class VanillaShop {
  public static readonly PotteryPieceList CultivatorPieces =
      new PotteryPieceList()
          .AddPiece(
              "vines",
              Workstation.None,
              [ (Item.Wood, 2) ]);
}
