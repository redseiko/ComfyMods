using BepInEx.Configuration;

using ComfyLib;

namespace LicenseToSkill {
  public class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }
    public static ConfigEntry<float> HardDeathCooldownOverride { get; private set; }
    public static ConfigEntry<float> SkillLossPercentOverride { get; private set; }

    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      HardDeathCooldownOverride =
          config.Bind(
              "OnDeath",
              "hardDeathCooldownOverride",
              20f,
              new ConfigDescription(
                  "Duration (in minutes) of the 'no skill loss' status effect after death.",
                  new AcceptableValueRange<float>(10f, 20f)));

      SkillLossPercentOverride =
          config.Bind(
              "OnDeath",
              "skillLossPercentOverride",
              1f,
              new ConfigDescription(
                  "Percentage of the skill's current level to lose on death.",
                  new AcceptableValueRange<float>(1f, 5f)));

      IsModEnabled.OnSettingChanged(LicenseToSkill.SetHardDeathCoolDown);

      HardDeathCooldownOverride.OnSettingChanged(LicenseToSkill.SetHardDeathCoolDown);
    }
  }
}
