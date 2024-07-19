namespace PotteryBarn;

using System.Collections.Generic;

public static class Requirements {
  public static readonly Dictionary<string, Dictionary<string, int>> HammerCreatorShopItems = new() {
     // Roots, Vines, and Glowing Mushroom
     {"root07", new Dictionary<string, int>() {
        {"ElderBark", 2 }}},
     {"root08", new Dictionary<string, int>() {
        {"ElderBark", 2 }}},
     {"root11", new Dictionary<string, int>() {
        {"ElderBark", 2 }}},
     {"root12", new Dictionary<string, int>() {
        {"ElderBark", 2 }}},
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

  public static readonly Dictionary<string, string> CraftingStationRequirements = new() {
    {"dvergrtown_wood_support", "blackforge" },
    {"dvergrprops_chair", "piece_workbench" },
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
