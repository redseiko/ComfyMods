namespace PotteryBarn;

public static class BuilderShop {
  public static readonly string NoStation = string.Empty;
  public const string BlackForge = "blackforge";
  public const string Forge = "forge";
  public const string Stonecutter = "piece_stonecutter";
  public const string Workbench = "piece_workbench";

  public const string BlackMarble = "BlackMarble";
  public const string Blackwood = "Blackwood";
  public const string Bronze = "Bronze";
  public const string Coal = "Coal";
  public const string Copper = "Copper";
  public const string Crystal = "Crystal";
  public const string FlametalNew = "FlametalNew";
  public const string Grausten = "Grausten";
  public const string JuteRed = "JuteRed";
  public const string LoxPelt = "LoxPelt";
  public const string PotShardRed = "Pot_Shard_Red";
  public const string Resin = "Resin";
  public const string WolfPelt = "WolfPelt";
  public const string Wood = "Wood";
  public const string YggdrasilWood = "YggdrasilWood";

  public static readonly PotteryPieceList HammerPieces =
      new PotteryPieceList()
          .Add(
              "Ashlands_Fortress_Floor",
              Stonecutter,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(Grausten, 4))
          .Add(
              "Ashlands_Fortress_Gate",
              Stonecutter,
              new PieceResourceList()
                  .Add(PotShardRed, 10)
                  .Add(Grausten, 10))
          .Add(
              "Ashlands_Fortress_Gate_Door",
              BlackForge,
              new PieceResourceList()
                  .Add(PotShardRed, 10)
                  .Add(FlametalNew, 5))
          .Add(
              "Ashlands_Fortress_Wall_Spikes",
              BlackForge,
              new PieceResourceList()
                  .Add(PotShardRed, 10)
                  .Add(Blackwood, 6)
                  .Add(FlametalNew, 6))
          .Add(
              "Ashlands_Wall_2x2",
              Stonecutter,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(Grausten, 6))
          .Add(
              "Ashlands_Wall_2x2_cornerR",
              Stonecutter,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(Grausten, 6))
          .Add(
              "Ashlands_Wall_2x2_cornerR_top",
              Stonecutter,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(Grausten, 6))
          .Add(
              "Ashlands_Wall_2x2_edge",
              Stonecutter,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(Grausten, 6))
          .Add(
              "Ashlands_Wall_2x2_edge_top",
              Stonecutter,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(Grausten, 6))
          .Add(
              "Ashlands_Wall_2x2_edge2",
              Stonecutter,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(Grausten, 6))
          .Add(
              "Ashlands_Wall_2x2_edge2_top",
              Stonecutter,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(Grausten, 6))
          .Add(
              "Ashlands_Wall_2x2_top",
              Stonecutter,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(Grausten, 6))
          .Add(
              "blackmarble_altar_crystal_broken",
              Stonecutter,
              new PieceResourceList()
                  .Add(PotShardRed, 5)
                  .Add(Crystal, 8))
          .Add(
              "blackmarble_post01",
              Stonecutter,
              new PieceResourceList()
                  .Add(PotShardRed, 10)
                  .Add(BlackMarble, 7))
          .Add(
              "CharredBanner1",
              Workbench,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(Blackwood, 4)
                  .Add(JuteRed, 2))
          .Add(
              "CharredBanner2",
              Workbench,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(Blackwood, 3)
                  .Add(JuteRed, 1))
          .Add(
              "CharredBanner3",
              Workbench,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(Blackwood, 2)
                  .Add(JuteRed, 1))
          .Add(
              "dvergrprops_barrel",
              Workbench,
              new PieceResourceList()
                  .Add(PotShardRed, 5)
                  .Add(YggdrasilWood, 25)
                  .Add(Copper, 5)
                  .Add(Resin, 10))
          .Add(
              "dvergrprops_bed",
              Workbench,
              new PieceResourceList()
                  .Add(PotShardRed, 5)
                  .Add(YggdrasilWood, 8)
                  .Add(Copper, 1)
                  .Add(WolfPelt, 2)
                  .Add(LoxPelt, 1))
          .Add(
              "dvergrprops_chair",
              Workbench,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(YggdrasilWood, 4)
                  .Add(Copper, 1))
          .Add(
              "dvergrprops_crate",
              Workbench,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(YggdrasilWood, 6))
          .Add(
              "dvergrprops_crate_long",
              Workbench,
              new PieceResourceList()
                  .Add(PotShardRed, 5)
                  .Add(YggdrasilWood, 10)
                  .Add(Copper, 1))
          .Add(
              "dvergrprops_shelf",
              Workbench,
              new PieceResourceList()
                  .Add(PotShardRed, 5)
                  .Add(YggdrasilWood, 15)
                  .Add(Copper, 4))
          .Add(
              "dvergrprops_stool",
              Workbench,
              new PieceResourceList()
                  .Add(PotShardRed, 1)
                  .Add(YggdrasilWood, 2)
                  .Add(Copper, 1))
          .Add(
              "dvergrprops_table",
              Workbench,
              new PieceResourceList()
                  .Add(PotShardRed, 5)
                  .Add(YggdrasilWood, 10)
                  .Add(Copper, 2))
          .Add(
              "dvergrtown_slidingdoor",
              Workbench,
              new PieceResourceList()
                  .Add(PotShardRed, 10)
                  .Add(BlackMarble, 10))
          .Add(
              "MountainKit_brazier",
              Forge,
              new PieceResourceList()
                  .Add(PotShardRed, 5)
                  .Add(Coal, 4)
                  .Add(Bronze, 2))
          .Add(
              "MountainKit_brazier_blue",
              Forge,
              new PieceResourceList()
                  .Add(PotShardRed, 5)
                  .Add(Coal, 4)
                  .Add(Bronze, 2))
          .Add(
              "MountainKit_brazier_purple",
              Forge,
              new PieceResourceList()
                  .Add(PotShardRed, 5)
                  .Add(Coal, 4)
                  .Add(Bronze, 2))
          .Add(
              "MountainKit_wood_gate",
              Workbench,
              new PieceResourceList()
                  .Add(PotShardRed, 5)
                  .Add(Wood, 10)
                  .Add(Bronze, 2))
          .Add(
              "TreasureChest_dvergrtower",
              Workbench,
              new PieceResourceList()
                  .Add(PotShardRed, 5)
                  .Add(YggdrasilWood, 7)
                  .Add(Copper, 7));
}
