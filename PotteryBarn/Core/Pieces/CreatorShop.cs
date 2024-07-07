namespace PotteryBarn;

using static PieceConstants;

public static class CreatorShop {
  public static readonly PotteryPieceList HammerPieces =
      new PotteryPieceList()
          .AddPiece(
              "asksvin_carrion",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 5),
                (Item.AsksvinCarrionNeck, 1),
                (Item.AsksvinCarrionRibcage, 1),
                (Item.AsksvinCarrionSkull, 1) ])
          .AddPiece(
              "asksvin_carrion2",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 5),
                (Item.AsksvinCarrionNeck, 1),
                (Item.AsksvinCarrionRibcage, 1),
                (Item.AsksvinCarrionSkull, 1) ])
          .AddPiece(
              "Charred_altar_bellfragment",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 3),
                (Item.BellFragment, 3),
                (Item.Grausten, 5),
                (Item.Crystal, 5) ])
          .AddPiece(
              "CreepProp_egg_hanging01",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 3),
                (Item.RoyalJelly, 20),
                (Item.TrophySeeker, 1) ])
          .AddPiece(
              "CreepProp_entrance1",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 2),
                (Item.RoyalJelly, 30) ])
          .AddPiece(
              "CreepProp_hanging01",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 2),
                (Item.RoyalJelly, 20) ])
          .AddPiece(
              "CreepProp_wall01",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 2),
                (Item.RoyalJelly, 20) ])
          .AddPiece(
              "dverger_demister",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 8),
                (Item.Wisp, 20) ])
          .AddPiece(
              "dverger_demister_large",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 30),
                (Item.Wisp, 50) ])
          .AddPiece(
              "dvergrprops_hooknchain",
              Workstation.BlackForge,
              [ (Item.MushroomBlue, 1),
                (Item.Iron, 2),
                (Item.Chain, 3) ])
          .AddPiece(
              "dvergrtown_creep_door",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 3),
                (Item.RoyalJelly, 50) ])
          .AddPiece(
              "dvergrtown_wood_crane",
              Workstation.BlackForge,
              [ (Item.MushroomBlue, 5),
                (Item.YggdrasilWood, 20),
                (Item.Copper, 4),
                (Item.BronzeNails, 20) ])
          .AddPiece(
              "dvergrtown_wood_support",
              Workstation.BlackForge,
              [ (Item.MushroomBlue, 3),
                (Item.YggdrasilWood, 20) ])
          .AddPiece(
              "fader_bellholder",
              Workstation.BlackForge,
              [ (Item.MushroomBlue, 3),
                (Item.FlametalNew, 10) ]);
}
