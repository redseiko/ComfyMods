namespace PotteryBarn;

using System.Collections.Generic;

public class Requirements {
  public static readonly Dictionary<string, Dictionary<string, int>> HammerCreatorShopItems = new() {
    // Goblin items
    {"goblin_banner", new Dictionary<string, int>() {
        {"FineWood", 2 },
        {"LeatherScraps", 6 },
        {"Bloodbag", 2 },
        {"BoneFragments", 2 },
        {"MushroomBlue", 1 }}},
    {"goblin_fence", new Dictionary<string, int>() {
        {"Wood", 4 },
        {"BoneFragments", 8},
        {"MushroomBlue", 1 }}},
    {"goblin_pole", new Dictionary<string, int>() {
        {"Wood", 2 },
        {"BoneFragments", 4},
        {"MushroomBlue", 1 }}},
     {"goblin_pole_small", new Dictionary<string, int>() {
        {"Wood", 1 },
        {"BoneFragments", 2},
        {"MushroomBlue", 1 }}},
     {"goblin_roof_45d", new Dictionary<string, int>() {
        {"Wood", 2 },
        {"DeerHide", 2 },
        {"BoneFragments", 8 },
        {"MushroomBlue", 1 }}},
     {"goblin_roof_45d_corner", new Dictionary<string, int>() {
        {"Wood", 2 },
        {"DeerHide", 2 },
        {"BoneFragments", 8 },
        {"MushroomBlue", 1 }}},
     {"goblin_roof_cap", new Dictionary<string, int>() {
        {"Wood", 10 },
        {"DeerHide", 6 },
        {"BoneFragments", 12 },
        {"MushroomBlue", 4 }}},
     {"goblin_stairs", new Dictionary<string, int>() {
        {"Wood", 2 },
        {"BoneFragments", 4 },
        {"MushroomBlue", 1 }}},
     {"goblin_stepladder", new Dictionary<string, int>() {
        {"Wood", 2 },
        {"BoneFragments", 4 },
        {"MushroomBlue", 1 }}},
     {"goblin_woodwall_1m", new Dictionary<string, int>() {
        {"Wood", 2 },
        {"BoneFragments", 4 },
        {"MushroomBlue", 1 }}},
     {"goblin_woodwall_2m", new Dictionary<string, int>() {
        {"Wood", 2 },
        {"BoneFragments", 4 },
        {"MushroomBlue", 1 }}},
     {"goblin_woodwall_2m_ribs", new Dictionary<string, int>() {
        {"Wood", 2 },
        {"BoneFragments", 4 },
        {"MushroomBlue", 1 }}},

    // Statues, Stone Chests, and Skulls
     {"Skull1", new Dictionary<string, int>() {
        {"BoneFragments", 10 },
        {"MushroomBlue", 1 }}},
     {"Skull2", new Dictionary<string, int>() {
        {"BoneFragments", 50 },
        {"MushroomBlue", 10 }}},
     {"StatueCorgi", new Dictionary<string, int>() {
        {"Stone", 20 },
        {"MushroomBlue", 5 }}},
     {"StatueDeer", new Dictionary<string, int>() {
        {"Stone", 20 },
        {"MushroomBlue", 5 }}},
     {"StatueEvil", new Dictionary<string, int>() {
        {"Stone", 20 },
        {"MushroomBlue", 5 }}},
     {"StatueHare", new Dictionary<string, int>() {
        {"Stone", 20 },
        {"MushroomBlue", 5 }}},
     {"StatueSeed", new Dictionary<string, int>() {
        {"Stone", 20 },
        {"MushroomBlue", 5 }}},
     {"stonechest", new Dictionary<string, int>() {
        {"Stone", 20 },
        {"MushroomBlue", 10 }}},

     // Roots, Vines, and Glowing Mushroom
     {"root07", new Dictionary<string, int>() {
        {"ElderBark", 2 }}},
     {"root08", new Dictionary<string, int>() {
        {"ElderBark", 2 }}},
     {"root11", new Dictionary<string, int>() {
        {"ElderBark", 2 }}},
     {"root12", new Dictionary<string, int>() {
        {"ElderBark", 2 }}},

     // Dvergr Items
     {"trader_wagon_destructable", new Dictionary<string, int>() {
        {"YggdrasilWood", 50 },
        {"BronzeNails", 80 },
        {"Tar", 10 },
        {"MushroomBlue", 10 }}},

     {"dvergrtown_wood_support", new Dictionary<string, int>() {
        {"YggdrasilWood", 20 },
        {"MushroomBlue", 3 }}},

     {"dvergrprops_chair", new Dictionary<string, int>() {
        {"YggdrasilWood", 4 },
        {"Copper", 1 },
        {"MushroomBlue", 1 }}},

     {"dvergrprops_bed", new Dictionary<string, int>() {
        {"YggdrasilWood", 8 },
        {"Copper", 1 },
        {"WolfPelt", 2 },
        {"LoxPelt", 1 },
        {"MushroomBlue", 1 }}},

     {"dverger_demister", new Dictionary<string, int>() {
        {"Wisp", 20 },
        {"MushroomBlue", 8 }}},

     {"dverger_demister_large", new Dictionary<string, int>() {
        {"Wisp", 50 },
        {"MushroomBlue", 30 }}}
  };

  public static readonly Dictionary<string, Dictionary<string, int>> MiscPrefabs =
    new() {
      { "portal", new() {
        { "Stone", 30 },
        { "SurtlingCore", 2},
        { "GreydwarfEye", 10} }},
      { "CastleKit_brazier", new() {
        { "Bronze", 5 },
        { "Coal", 2},
        { "WolfClaw", 3} }}
    };

  public static readonly Dictionary<string, Dictionary<string, int>> CultivatorCreatorShopItems =
      new() {
        { "vines",
          new() { {"Wood", 2 }} },
        { "GlowingMushroom",
          new() {
            { "MushroomYellow", 3 },
            { "MushroomBlue", 1 }} }
      };

  //public static readonly Dictionary<string, Dictionary<string, int>> cultivatorCreatorShopItems = new() {
  //   // Natural Items
  //   {"Bush01", new Dictionary<string, int>() {
  //      {"Wood", 2 }}},
  //   {"Bush01_heath", new Dictionary<string, int>() {
  //      {"Wood", 2 }}},
  //   {"Bush02_en", new Dictionary<string, int>() {
  //      {"Wood", 3 }}},
  //   {"shrub_2", new Dictionary<string, int>() {
  //      {"Wood", 2 }}},
  //   {"shrub_2_heath", new Dictionary<string, int>() {
  //      {"Wood", 2 }}},
  //   {"marker01", new Dictionary<string, int>() {
  //      {"Stone", 10 }}},
  //   {"marker02", new Dictionary<string, int>() {
  //      {"Stone", 10 }}},
  //   {"Rock_3", new Dictionary<string, int>() {
  //      {"Stone", 30 }}},
  //   {"Rock_4", new Dictionary<string, int>() {
  //      {"Stone", 30 }}},
  //   {"Rock_7", new Dictionary<string, int>() {
  //      {"Stone", 10 }}},
  //   {"highstone", new Dictionary<string, int>() {
  //      {"Stone", 50 }}},
  //   {"widestone", new Dictionary<string, int>() {
  //      {"Stone", 50 }}}
  //};

  public static readonly Dictionary<string, string> craftingStationRequirements = new() {
    {"dvergrtown_wood_support", "blackforge" },
    {"dvergrprops_chair", "blackforge" },
    {"dvergrprops_bed", "blackforge" },
    {"goblin_banner", "piece_workbench" },
    {"goblin_fence", "piece_workbench" },
    {"goblin_pole", "piece_workbench" },
    {"goblin_pole_small", "piece_workbench" },
    {"goblin_roof_45d", "piece_workbench" },
    {"goblin_roof_45d_corner", "piece_workbench" },
    {"goblin_roof_cap", "piece_workbench" },
    {"goblin_stairs", "piece_workbench" },
    {"goblin_stepladder", "piece_workbench" },
    {"goblin_woodwall_1m", "piece_workbench" },
    {"goblin_woodwall_2m", "piece_workbench" },
    {"goblin_woodwall_2m_ribs", "piece_workbench" },
    {"portal", "piece_stonecutter" },
    {"Skull1", "piece_workbench" },
    {"Skull2", "piece_workbench" },
    {"StatueCorgi", "piece_stonecutter" },
    {"StatueDeer", "piece_stonecutter" },
    {"StatueEvil", "piece_stonecutter" },
    {"StatueHare", "piece_stonecutter" },
    {"StatueSeed", "piece_stonecutter" },
    {"stonechest", "piece_stonecutter" },
    {"trader_wagon_destructable", "piece_workbench" }
  };
}
