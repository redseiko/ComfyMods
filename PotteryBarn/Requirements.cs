using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PotteryBarn {
  public class Requirements {
    public static readonly Dictionary<string, Dictionary<string, int>> creatorShopItems = new Dictionary<string, Dictionary<string, int>>() {
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

       // Natural Items
       {"Bush01", new Dictionary<string, int>() {
          {"Wood", 2 }}},
       {"Bush01_heath", new Dictionary<string, int>() {
          {"Wood", 2 }}},
       {"Bush02_en", new Dictionary<string, int>() {
          {"Wood", 3 }}},
       {"shrub_2", new Dictionary<string, int>() {
          {"Wood", 2 }}},
       {"shrub_2_heath", new Dictionary<string, int>() {
          {"Wood", 2 }}},
       {"vines", new Dictionary<string, int>() {
          {"Wood", 2 }}},
       {"root07", new Dictionary<string, int>() {
          {"ElderBark", 2 }}},
       {"root08", new Dictionary<string, int>() {
          {"ElderBark", 2 }}},
       {"root11", new Dictionary<string, int>() {
          {"ElderBark", 2 }}},
       {"root12", new Dictionary<string, int>() {
          {"ElderBark", 2 }}},
       {"marker01", new Dictionary<string, int>() {
          {"Stone", 10 }}},
       {"marker02", new Dictionary<string, int>() {
          {"Stone", 10 }}},
       {"Rock_3", new Dictionary<string, int>() {
          {"Stone", 30 }}},
       {"Rock_4", new Dictionary<string, int>() {
          {"Stone", 30 }}},
       {"Rock_7", new Dictionary<string, int>() {
          {"Stone", 10 }}},
       {"highstone", new Dictionary<string, int>() {
          {"Stone", 10 }}},
       {"widestone", new Dictionary<string, int>() {
          {"Stone", 10 }}},
       {"GlowingMushroom", new Dictionary<string, int>() {
          {"MushroomYellow", 3 },
          {"MushroomBlue", 1 }}},
       
       // Statues and Skulls
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
    };
  }
}
