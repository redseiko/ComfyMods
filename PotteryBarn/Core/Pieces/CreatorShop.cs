namespace PotteryBarn;

public static class CreatorShop {
  public static readonly string NoStation = string.Empty;
  public const string BlackForge = "blackforge";
  public const string Forge = "forge";
  public const string Stonecutter = "piece_stonecutter";
  public const string Workbench = "piece_workbench";

  public const string AsksvinCarrionNeck = "AsksvinCarrionNeck";
  public const string AsksvinCarrionRibcage = "AsksvinCarrionRibcage";
  public const string AsksvinCarrionSkull = "AsksvinCarrionSkull";
  public const string MushroomBlue = "MushroomBlue";

  public static readonly PotteryPieceList HammerPieces =
      new PotteryPieceList()
          .Add(
              "asksvin_carrion",
              Workbench,
              new PieceResourceList()
                  .Add(MushroomBlue, 5)
                  .Add(AsksvinCarrionNeck, 1)
                  .Add(AsksvinCarrionRibcage, 1)
                  .Add(AsksvinCarrionSkull, 1))
          .Add(
              "asksvin_carrion2",
              Workbench,
              new PieceResourceList()
                  .Add(MushroomBlue, 5)
                  .Add(AsksvinCarrionNeck, 1)
                  .Add(AsksvinCarrionRibcage, 1)
                  .Add(AsksvinCarrionSkull, 1));
}
