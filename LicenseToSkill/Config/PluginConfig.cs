namespace LicenseToSkill;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }
  public static ConfigEntry<float> HardDeathCooldownOverride { get; private set; }
  public static ConfigEntry<float> SkillLossPercentOverride { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    HardDeathCooldownOverride =
        config.BindInOrder(
            "OnDeath",
            "hardDeathCooldownOverride",
            20f,
            "Duration (in minutes) of the 'no skill loss' status effect after death.",
             new AcceptableValueRange<float>(10f, 20f));

    SkillLossPercentOverride =
        config.BindInOrder(
            "OnDeath",
            "skillLossPercentOverride",
            1f,
            "Percentage of the skill's current level to lose on death.",
            new AcceptableValueRange<float>(1f, 5f));

    IsModEnabled.OnSettingChanged(StatusEffectUtils.SetHardDeathCoolDown);

    HardDeathCooldownOverride.OnSettingChanged(StatusEffectUtils.SetHardDeathCoolDown);
  }
}
