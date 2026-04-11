namespace ComfyLadders;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigFile CurrentConfig { get; private set; }
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<bool> AshlandSteepstairIsEligible { get; private set; }
  public static ConfigEntry<bool> GoblinStepladderIsEligible { get; private set; }
  public static ConfigEntry<bool> GraustenStoneLadderIsEligible { get; private set; }
  public static ConfigEntry<bool> WoodStepladderIsEligible { get; private set; }

  public static void BindConfig(ConfigFile config) {
    CurrentConfig = config;

    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    AshlandSteepstairIsEligible =
        config.BindInOrder(
            "Ladders",
            "ashlandSteepstairIsEligible",
            true,
            "If true, prefab `Ashland_Steepstair` is eligible for custom ladder movement.");

    GoblinStepladderIsEligible =
        config.BindInOrder(
            "Ladders",
            "goblinStepladderIsEligible",
            false,
            "If true, prefab `goblin_stepladder` is eligible for custom ladder movement.");

    GraustenStoneLadderIsEligible =
        config.BindInOrder(
            "Ladders",
            "graustenStoneLadderIsEligible",
            true,
            "If true, prefab `Piece_grausten_stone_ladder` is eligible for custom ladder movement.");

    WoodStepladderIsEligible =
        config.BindInOrder(
            "Ladders",
            "woodStepladderIsEligible",
            true,
            "If true, prefab `wood_stepladder` is eligible for custom ladder movement.");
  }
}
