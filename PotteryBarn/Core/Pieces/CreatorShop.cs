namespace PotteryBarn;

using static PieceConstants;

public static class CreatorShop {
  public static readonly PotteryPieceList CultivatorPieces =
      new PotteryPieceList()
          .AddPiece(
              "GlowingMushroom",
              Workstation.None,
              [ (Item.MushroomBlue, 1),
                (Item.MushroomYellow, 3) ])
          .AddPiece(
              "marker01",
              Workstation.None,
              [ (Item.MushroomBlue, 1),
                (Item.Stone, 5) ]);

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
                (Item.FlametalNew, 10) ])
          .AddPiece(
              "goblin_banner",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.FineWood, 2),
                (Item.LeatherScraps, 6),
                (Item.Bloodbag, 2),
                (Item.BoneFragments, 2) ])
          .AddPiece(
              "goblin_fence",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Wood, 4),
                (Item.BoneFragments, 8) ])
          .AddPiece(
              "goblin_pole",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Wood, 2),
                (Item.BoneFragments, 4) ])
          .AddPiece(
              "goblin_pole_small",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Wood, 1),
                (Item.BoneFragments, 2) ])
          .AddPiece(
              "goblin_roof_45d",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Wood, 2),
                (Item.DeerHide, 2),
                (Item.BoneFragments, 8) ])
          .AddPiece(
              "goblin_roof_45d_corner",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Wood, 2),
                (Item.DeerHide, 2),
                (Item.BoneFragments, 8) ])
          .AddPiece(
              "goblin_roof_cap",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 4),
                (Item.Wood, 10),
                (Item.DeerHide, 6),
                (Item.BoneFragments, 12) ])
          .AddPiece(
              "goblin_stairs",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Wood, 2),
                (Item.BoneFragments, 4) ])
          .AddPiece(
              "goblin_stepladder",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Wood, 2),
                (Item.BoneFragments, 4) ])
          .AddPiece(
              "goblin_woodwall_1m",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Wood, 2),
                (Item.BoneFragments, 4) ])
          .AddPiece(
              "goblin_woodwall_2m",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Wood, 4),
                (Item.BoneFragments, 8) ])
          .AddPiece(
              "goblin_woodwall_2m_ribs",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Wood, 4),
                (Item.BoneFragments, 8) ])
          .AddPiece(
              "goblinking_totemholder",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.BoneFragments, 4) ])
          .AddPiece(
              "GraveStone_CharredTwitcherNest",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Grausten, 5),
                (Item.CharcoalResin, 1) ])
          .AddPiece(
              "GraveStone_Elite_CharredTwitcherNest",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Grausten, 5),
                (Item.CharcoalResin, 1) ])
          .AddPiece(
              "Skull1",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.BoneFragments, 10) ])
          .AddPiece(
              "Skull2",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 10),
                (Item.BoneFragments, 50) ])
          .AddPiece(
              "StatueCorgi",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 5),
                (Item.Stone, 20) ])
          .AddPiece(
              "StatueDeer",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 5),
                (Item.Stone, 20) ])
          .AddPiece(
              "StatueEvil",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 5),
                (Item.Stone, 20) ])
          .AddPiece(
              "StatueHare",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 5),
                (Item.Stone, 20) ])
          .AddPiece(
              "StatueSeed",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 5),
                (Item.Stone, 20) ])
          .AddPiece(
              "stonechest",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 10),
                (Item.Stone, 20) ])
          .AddPiece(
              "trader_wagon_destructable",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 10),
                (Item.YggdrasilWood, 50),
                (Item.BronzeNails, 80),
                (Item.Tar, 10) ])
          .AddPiece(
              "TreasureChest_dvergr_loose_stone",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 10),
                (Item.BlackMarble, 20) ])
          .AddPiece(
              "TreasureChest_dvergrtown",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 10),
                (Item.YggdrasilWood, 10),
                (Item.Copper, 2) ])
          .AddPiece(
              "volture_strawpile",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 2),
                (Item.Wood, 10),
                (Item.CharredBone, 10) ]);
}
