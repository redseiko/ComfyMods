namespace PotteryBarn;

using static PieceConstants;

public static class CreativeShop {
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
              [ (Item.Stone, 5) ]);

  public static readonly PotteryPieceList HammerPieces =
      new PotteryPieceList()
          // A
          .AddPiece(
              "Ashlands_Fortress_Floor",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Grausten, 4) ])
          .AddPiece(
              "Ashlands_Fortress_Gate",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 2),
                (Item.Grausten, 15) ])
          .AddPiece(
              "Ashlands_Fortress_Gate_Door",
              Workstation.BlackForge,
              [ (Item.MushroomBlue, 2),
                (Item.FlametalNew, 6),
                (Item.Bronze, 6) ])
          .AddPiece(
              "Ashlands_Fortress_Wall_Spikes",
              Workstation.BlackForge,
              [ (Item.MushroomBlue, 1),
                (Item.FlametalNew, 2),
                (Item.Bronze, 2) ])
          .AddPiece(
              "Ashlands_Wall_2x2",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Grausten, 6) ])
          .AddPiece(
              "Ashlands_Wall_2x2_cornerR",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Grausten, 6) ])
          .AddPiece(
              "Ashlands_Wall_2x2_cornerR_top",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Grausten, 6) ])
          .AddPiece(
              "Ashlands_Wall_2x2_edge",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Grausten, 6) ])
          .AddPiece(
              "Ashlands_Wall_2x2_edge_top",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Grausten, 6) ])
          .AddPiece(
              "Ashlands_Wall_2x2_edge2",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Grausten, 6) ])
          .AddPiece(
              "Ashlands_Wall_2x2_edge2_top",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Grausten, 6) ])
          .AddPiece(
              "Ashlands_Wall_2x2_top",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Grausten, 6) ])
          .AddPiece(
              "asksvin_carrion",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.AsksvinCarrionNeck, 1),
                (Item.AsksvinCarrionRibcage, 1),
                (Item.AsksvinCarrionSkull, 1) ])
          .AddPiece(
              "asksvin_carrion2",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.AsksvinCarrionNeck, 1),
                (Item.AsksvinCarrionRibcage, 1),
                (Item.AsksvinCarrionSkull, 1) ])
          // B
          .AddPiece(
              "blackmarble_altar_crystal_broken",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Crystal, 8) ])
          .AddPiece(
              "blackmarble_post01",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.BlackMarble, 15) ])
          // C
          .AddPiece(
              "Charred_altar_bellfragment",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 3),
                (Item.BellFragment, 3),
                (Item.Grausten, 5),
                (Item.Crystal, 5) ])
          .AddPiece(
              "CharredBanner1",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Blackwood, 4),
                (Item.JuteRed, 2) ])
          .AddPiece(
              "CharredBanner2",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Blackwood, 3),
                (Item.JuteRed, 1) ])
          .AddPiece(
              "CharredBanner3",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Blackwood, 2),
                (Item.JuteRed, 1) ])
          .AddPiece(
              "CreepProp_egg_hanging01",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.RoyalJelly, 20) ])
          .AddPiece(
              "CreepProp_entrance1",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.RoyalJelly, 20) ])
          .AddPiece(
              "CreepProp_hanging01",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.RoyalJelly, 10) ])
          .AddPiece(
              "CreepProp_wall01",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.RoyalJelly, 10) ])
          // D
          .AddPiece(
              "dverger_demister",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 2),
                (Item.Iron, 2),
                (Item.Wisp, 20) ])
          .AddPiece(
              "dverger_demister_large",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 10),
                (Item.Iron, 4),
                (Item.Wisp, 50) ])
          .AddPiece(
              "dvergrprops_barrel",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.YggdrasilWood, 15),
                (Item.Copper, 4) ])
          .AddPiece(
              "dvergrprops_bed",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.YggdrasilWood, 4),
                (Item.Copper, 2),
                (Item.LoxPelt, 2) ])
          .AddPiece(
              "dvergrprops_chair",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.YggdrasilWood, 2),
                (Item.Copper, 1) ])
          .AddPiece(
              "dvergrprops_crate",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.YggdrasilWood, 4) ])
          .AddPiece(
              "dvergrprops_crate_long",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.YggdrasilWood, 4),
                (Item.Copper, 1) ])
          .AddPiece(
              "dvergrprops_hooknchain",
              Workstation.BlackForge,
              [ (Item.MushroomBlue, 1),
                (Item.Chain, 10) ])
          .AddPiece(
              "dvergrprops_shelf",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.YggdrasilWood, 6),
                (Item.Copper, 2) ])
          .AddPiece(
              "dvergrprops_stool",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.YggdrasilWood, 2),
                (Item.Copper, 1) ])
          .AddPiece(
              "dvergrprops_table",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.YggdrasilWood, 4),
                (Item.Copper, 2) ])
          .AddPiece(
              "dvergrtown_creep_door",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 2),
                (Item.RoyalJelly, 20) ])
          .AddPiece(
              "dvergrtown_slidingdoor",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 2),
                (Item.BlackMarble, 6),
                (Item.Copper, 6)])
          .AddPiece(
              "dvergrtown_wood_crane",
              Workstation.BlackForge,
              [ (Item.MushroomBlue, 2),
                (Item.YggdrasilWood, 20),
                (Item.Copper, 4) ])
          .AddPiece(
              "dvergrtown_wood_support",
              Workstation.BlackForge,
              [ (Item.MushroomBlue, 2),
                (Item.YggdrasilWood, 20) ])
          .AddPiece(
              "fader_bellholder",
              Workstation.BlackForge,
              [ (Item.MushroomBlue, 1),
                (Item.FlametalNew, 5) ])
          // G
          .AddPiece(
              "goblin_banner",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.LeatherScraps, 6),
                (Item.Bloodbag, 2),
                (Item.BoneFragments, 4) ])
          .AddPiece(
              "goblin_fence",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.BoneFragments, 8) ])
          .AddPiece(
              "goblin_pole",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.BoneFragments, 8) ])
          .AddPiece(
              "goblin_pole_small",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.BoneFragments, 4) ])
          .AddPiece(
              "goblin_roof_45d",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.DeerHide, 2),
                (Item.BoneFragments, 8) ])
          .AddPiece(
              "goblin_roof_45d_corner",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.DeerHide, 2),
                (Item.BoneFragments, 8) ])
          .AddPiece(
              "goblin_roof_cap",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 2),
                (Item.DeerHide, 6),
                (Item.BoneFragments, 12) ])
          .AddPiece(
              "goblin_stairs",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.BoneFragments, 4) ])
          .AddPiece(
              "goblin_stepladder",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.BoneFragments, 4) ])
          .AddPiece(
              "goblin_woodwall_1m",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.BoneFragments, 4) ])
          .AddPiece(
              "goblin_woodwall_2m",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.BoneFragments, 8) ])
          .AddPiece(
              "goblin_woodwall_2m_ribs",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
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
          // M
          .AddPiece(
              "MountainKit_brazier",
              Workstation.Forge,
              [ (Item.MushroomBlue, 1),
                (Item.Coal, 2),
                (Item.Bronze, 2) ])
          .AddPiece(
              "MountainKit_brazier_blue",
              Workstation.Forge,
              [ (Item.MushroomBlue, 1),
                (Item.Coal, 2),
                (Item.Bronze, 2) ])
          .AddPiece(
              "MountainKit_brazier_purple",
              Workstation.Forge,
              [ (Item.MushroomBlue, 1),
                (Item.Coal, 2),
                (Item.Bronze, 2) ])
          .AddPiece(
              "MountainKit_wood_gate",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Wood, 10),
                (Item.Iron, 3) ])
          // P
          .AddPiece(
              "piece_pot1_red",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.CharcoalResin, 1) ])
          .AddPiece(
              "piece_pot2_red",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.CharcoalResin, 1) ])
          .AddPiece(
              "piece_pot3_red",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.CharcoalResin, 1) ])
          // S
          .AddPiece(
              "Skull1",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.TrophySkeleton, 1) ])
          .AddPiece(
              "Skull2",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 2),
                (Item.BoneFragments, 50),
                (Item.TrophySkeleton, 1) ])
          .AddPiece(
              "StatueCorgi",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Stone, 10) ])
          .AddPiece(
              "StatueDeer",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Stone, 10) ])
          .AddPiece(
              "StatueEvil",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Stone, 10) ])
          .AddPiece(
              "StatueHare",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Stone, 10) ])
          .AddPiece(
              "StatueSeed",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 1),
                (Item.Stone, 10) ])
          .AddPiece(
              "stonechest",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 2),
                (Item.Stone, 20) ])
          // T
          .AddPiece(
              "trader_wagon_destructable",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 5),
                (Item.YggdrasilWood, 30),
                (Item.BronzeNails, 40),
                (Item.Tar, 10) ])
          .AddPiece(
              "TreasureChest_dvergr_loose_stone",
              Workstation.Stonecutter,
              [ (Item.MushroomBlue, 2),
                (Item.BlackMarble, 20) ])
          .AddPiece(
              "TreasureChest_dvergrtower",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 2),
                (Item.YggdrasilWood, 10),
                (Item.Copper, 2) ])
          .AddPiece(
              "TreasureChest_dvergrtown",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 2),
                (Item.YggdrasilWood, 10),
                (Item.Copper, 2) ])
          // V
          .AddPiece(
              "volture_strawpile",
              Workstation.Workbench,
              [ (Item.MushroomBlue, 1),
                (Item.Barley, 20),
                (Item.CharredBone, 10) ]);
}
