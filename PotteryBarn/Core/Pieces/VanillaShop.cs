namespace PotteryBarn;

using static PieceConstants;

public static class VanillaShop {
  public static readonly PotteryPieceList CultivatorPieces =
      new PotteryPieceList()
          .AddPiece(
              "vines",
              Workstation.None,
              [ (Item.Wood, 2) ]);

  public static readonly PotteryPieceList BuildingPieces =
      new PotteryPieceList()
          .AddPiece(
              "Ashlands_floor_large",
              Workstation.Stonecutter,
              [ (Item.Grausten, 32) ])
          .AddPiece(
              "Ashlands_StairsBroad",
              Workstation.Stonecutter,
              [ (Item.Grausten, 24) ])
          .AddPiece(
              "ashwood_arch_top",
              Workstation.Workbench,
              [ (Item.Blackwood, 2) ])
          .AddPiece(
              "blackmarble_2x2_enforced",
              Workstation.BlackForge,
              [ (Item.BlackMarble, 8),
                (Item.Copper, 4) ])
          .AddPiece(
              "blackmarble_base_2",
              Workstation.Stonecutter,
              [ (Item.BlackMarble, 10) ])
          .AddPiece(
              "blackmarble_column_3",
              Workstation.Stonecutter,
              [ (Item.BlackMarble, 24) ])
          .AddPiece(
              "blackmarble_floor_large",
              Workstation.Stonecutter,
              [ (Item.BlackMarble, 80) ])
          .AddPiece(
              "blackmarble_head_big01",
              Workstation.BlackForge,
              [ (Item.BlackMarble, 8) ])
          .AddPiece(
              "blackmarble_head_big02",
              Workstation.BlackForge,
              [ (Item.BlackMarble, 8) ])
          .AddPiece(
              "blackmarble_head01",
              Workstation.BlackForge,
              [ (Item.Copper, 4) ])
          .AddPiece(
              "blackmarble_head02",
              Workstation.BlackForge,
              [ (Item.Copper, 4) ] )
          .AddPiece(
              "blackmarble_out_2",
              Workstation.Stonecutter,
              [ (Item.BlackMarble, 8) ])
          .AddPiece(
              "blackmarble_slope_1x2",
              Workstation.Stonecutter,
              [ (Item.BlackMarble, 3) ])
          .AddPiece(
              "blackmarble_tile_floor_1x1",
              Workstation.Stonecutter,
              [ (Item.BlackMarble, 2) ])
          .AddPiece(
              "blackmarble_tile_floor_2x2",
              Workstation.Stonecutter,
              [ (Item.BlackMarble, 4) ])
          .AddPiece(
              "blackmarble_tile_wall_2x4",
              Workstation.Stonecutter,
              [ (Item.BlackMarble, 8) ])
          .AddPiece(
              "dvergrprops_banner",
              Workstation.Workbench,
              [ (Item.YggdrasilWood, 2),
                (Item.JuteBlue, 4) ])
          .AddPiece(
              "dvergrprops_curtain",
              Workstation.Workbench,
              [ (Item.YggdrasilWood, 2),
                (Item.JuteBlue, 4) ])
          .AddPiece(
              "dvergrprops_lantern_standing",
              Workstation.BlackForge,
              [ (Item.Lantern, 1)] )
          .AddPiece(
              "dvergrprops_wood_beam",
              Workstation.Workbench,
              [ (Item.YggdrasilWood, 12) ])
          .AddPiece(
              "dvergrprops_wood_floor",
              Workstation.Workbench,
              [ (Item.YggdrasilWood, 2) ])
          .AddPiece(
              "dvergrprops_wood_pole",
              Workstation.BlackForge,
              [ (Item.YggdrasilWood, 8),
                (Item.Copper, 4) ])
          .AddPiece(
              "dvergrprops_wood_stair",
              Workstation.Workbench,
              [ (Item.YggdrasilWood, 2) ])
          .AddPiece(
              "dvergrprops_wood_wall",
              Workstation.BlackForge,
              [ (Item.YggdrasilWood, 32),
                (Item.Copper, 16) ])
          .AddPiece(
              "dvergrtown_stair_corner_wood_left",
              Workstation.BlackForge,
              [ (Item.YggdrasilWood, 6),
                (Item.Copper, 3) ])
          .AddPiece(
              "metalbar_1x2",
              Workstation.Stonecutter,
              [ (Item.Copper, 8) ])
          .AddPiece(
              "piece_dvergr_pole",
              Workstation.BlackForge,
              [ (Item.YggdrasilWood, 2),
                (Item.Copper, 1) ])
          .AddPiece(
              "piece_dvergr_wood_door",
              Workstation.BlackForge,
              [ (Item.YggdrasilWood, 12),
                (Item.Copper, 12) ])
          .AddPiece(
              "piece_dvergr_wood_wall",
              Workstation.BlackForge,
              [ (Item.YggdrasilWood, 10),
                (Item.Copper, 5) ])
          .AddPiece(
              "root07",
              Workstation.None,
              [ (Item.ElderBark, 2) ])
          .AddPiece(
              "root08",
              Workstation.None,
              [ (Item.ElderBark, 2) ])
          .AddPiece(
              "root11",
              Workstation.None,
              [ (Item.ElderBark, 2) ])
          .AddPiece(
              "root12",
              Workstation.None,
              [ (Item.ElderBark, 2) ]);

  public static readonly PotteryPieceList FurniturePieces =
      new PotteryPieceList()
          .AddPiece(
              "piece_pot1_cracked",
              Workstation.Workbench,
              [ (Item.PotShardGreen, 4),
                (Item.CharcoalResin, 1),
                (Item.ProustitePowder, 1) ])
          .AddPiece(
              "piece_pot2_cracked",
              Workstation.Workbench,
              [ (Item.PotShardGreen, 5),
                (Item.CharcoalResin, 1),
                (Item.ProustitePowder, 1) ])
          .AddPiece(
              "piece_pot3_cracked",
              Workstation.Workbench,
              [ (Item.PotShardGreen, 3),
                (Item.CharcoalResin, 1),
                (Item.ProustitePowder, 1) ]);

  public static readonly PotteryPieceList MiscPieces =
      new PotteryPieceList()
          .AddPiece(
              "portal",
              Workstation.Stonecutter,
              [ (Item.Stone, 30),
                (Item.SurtlingCore, 2),
                (Item.GreydwarfEye, 10) ])
          .AddPiece(
              "CastleKit_brazier",
              Workstation.Forge,
              [ (Item.Bronze, 5),
                (Item.Coal, 2),
                (Item.WolfClaw, 3) ]);
}
