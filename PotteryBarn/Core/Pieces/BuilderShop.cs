namespace PotteryBarn;

using static PieceConstants;

public static class BuilderShop {
  public static readonly PotteryPieceList HammerPieces =
      new PotteryPieceList()
          .AddPiece(
              "Ashlands_Fortress_Floor",
              Workstation.Stonecutter,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.Grausten, 4))
          .AddPiece(
              "Ashlands_Fortress_Gate",
              Workstation.Stonecutter,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 10)
                  .Add(Item.Grausten, 10))
          .AddPiece(
              "Ashlands_Fortress_Gate_Door",
              Workstation.BlackForge,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 10)
                  .Add(Item.FlametalNew, 5))
          .AddPiece(
              "Ashlands_Fortress_Wall_Spikes",
              Workstation.BlackForge,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 10)
                  .Add(Item.Blackwood, 6)
                  .Add(Item.FlametalNew, 6))
          .AddPiece(
              "Ashlands_Wall_2x2",
              Workstation.Stonecutter,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.Grausten, 6))
          .AddPiece(
              "Ashlands_Wall_2x2_cornerR",
              Workstation.Stonecutter,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.Grausten, 6))
          .AddPiece(
              "Ashlands_Wall_2x2_cornerR_top",
              Workstation.Stonecutter,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.Grausten, 6))
          .AddPiece(
              "Ashlands_Wall_2x2_edge",
              Workstation.Stonecutter,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.Grausten, 6))
          .AddPiece(
              "Ashlands_Wall_2x2_edge_top",
              Workstation.Stonecutter,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.Grausten, 6))
          .AddPiece(
              "Ashlands_Wall_2x2_edge2",
              Workstation.Stonecutter,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.Grausten, 6))
          .AddPiece(
              "Ashlands_Wall_2x2_edge2_top",
              Workstation.Stonecutter,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.Grausten, 6))
          .AddPiece(
              "Ashlands_Wall_2x2_top",
              Workstation.Stonecutter,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.Grausten, 6))
          .AddPiece(
              "blackmarble_altar_crystal_broken",
              Workstation.Stonecutter,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 5)
                  .Add(Item.Crystal, 8))
          .AddPiece(
              "blackmarble_post01",
              Workstation.Stonecutter,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 10)
                  .Add(Item.BlackMarble, 7))
          .AddPiece(
              "CharredBanner1",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.Blackwood, 4)
                  .Add(Item.JuteRed, 2))
          .AddPiece(
              "CharredBanner2",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.Blackwood, 3)
                  .Add(Item.JuteRed, 1))
          .AddPiece(
              "CharredBanner3",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.Blackwood, 2)
                  .Add(Item.JuteRed, 1))
          .AddPiece(
              "dvergrprops_barrel",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 5)
                  .Add(Item.YggdrasilWood, 25)
                  .Add(Item.Copper, 5)
                  .Add(Item.Resin, 10))
          .AddPiece(
              "dvergrprops_bed",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 5)
                  .Add(Item.YggdrasilWood, 8)
                  .Add(Item.Copper, 1)
                  .Add(Item.WolfPelt, 2)
                  .Add(Item.LoxPelt, 1))
          .AddPiece(
              "dvergrprops_chair",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.YggdrasilWood, 4)
                  .Add(Item.Copper, 1))
          .AddPiece(
              "dvergrprops_crate",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.YggdrasilWood, 6))
          .AddPiece(
              "dvergrprops_crate_long",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 5)
                  .Add(Item.YggdrasilWood, 10)
                  .Add(Item.Copper, 1))
          .AddPiece(
              "dvergrprops_shelf",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 5)
                  .Add(Item.YggdrasilWood, 15)
                  .Add(Item.Copper, 4))
          .AddPiece(
              "dvergrprops_stool",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 1)
                  .Add(Item.YggdrasilWood, 2)
                  .Add(Item.Copper, 1))
          .AddPiece(
              "dvergrprops_table",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 5)
                  .Add(Item.YggdrasilWood, 10)
                  .Add(Item.Copper, 2))
          .AddPiece(
              "dvergrtown_slidingdoor",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 10)
                  .Add(Item.BlackMarble, 10))
          .AddPiece(
              "MountainKit_brazier",
              Workstation.Forge,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 5)
                  .Add(Item.Coal, 4)
                  .Add(Item.Bronze, 2))
          .AddPiece(
              "MountainKit_brazier_blue",
              Workstation.Forge,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 5)
                  .Add(Item.Coal, 4)
                  .Add(Item.Bronze, 2))
          .AddPiece(
              "MountainKit_brazier_purple",
              Workstation.Forge,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 5)
                  .Add(Item.Coal, 4)
                  .Add(Item.Bronze, 2))
          .AddPiece(
              "MountainKit_wood_gate",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 5)
                  .Add(Item.Wood, 10)
                  .Add(Item.Bronze, 2))
          .AddPiece(
              "piece_pot1_red",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 4)
                  .Add(Item.CharcoalResin, 1))
          .AddPiece(
              "piece_pot2_red",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 5)
                  .Add(Item.CharcoalResin, 1))
          .AddPiece(
              "piece_pot3_red",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 3)
                  .Add(Item.CharcoalResin, 1))
          .AddPiece(
              "TreasureChest_dvergrtower",
              Workstation.Workbench,
              new PieceResourceList()
                  .Add(Item.PotShardRed, 5)
                  .Add(Item.YggdrasilWood, 7)
                  .Add(Item.Copper, 2));
}
