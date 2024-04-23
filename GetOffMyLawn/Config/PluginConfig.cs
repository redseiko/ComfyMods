namespace GetOffMyLawn;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<float> TargetPieceHealth { get; private set; }
  public static ConfigEntry<bool> RepairPiecesOnWardActivation { get; private set; }

  public static ConfigEntry<bool> EnablePieceHealthDamageThreshold { get; private set; }

  public static ConfigEntry<bool> ShowTopLeftMessageOnPieceRepair { get; private set; }
  public static ConfigEntry<bool> ShowRepairEffectOnWardActivation { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

    TargetPieceHealth =
        config.BindInOrder(
            "PieceValue",
            "targetPieceHealth",
            100_000_000_000_000_000f,
            "Target value to set piece health to when creating and repairing.");

    RepairPiecesOnWardActivation =
        config.BindInOrder(
            "Behaviour",
            "repairPiecesOnWardActivation",
            true,
            "IF set, will repair all Pieces in Ward range when a Ward is activated.");

    EnablePieceHealthDamageThreshold =
        config.BindInOrder(
            "Optimization",
            "enablePieceHealthDamageThreshold",
            true,
            "If piece health exceeds 100K, DO NOT execute ApplyDamage() or send WNTHealthChanged messages.");

    ShowTopLeftMessageOnPieceRepair =
        config.BindInOrder(
            "Indicators",
            "showTopLeftMessageOnPieceRepair",
            false,
            "Shows a message in the top-left message area on piece repair.");

    ShowRepairEffectOnWardActivation =
        config.BindInOrder(
            "Indicators",
            "showRepairEffectOnWardActivation",
            false,
            "Shows the repair effect on affected pieces when activating a ward.");
  }
}
