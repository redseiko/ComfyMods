namespace Dramamist;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigFile Config { get; private set; }
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<bool> ParticleMistReduceMotion { get; private set; }
  public static ConfigEntry<bool> ParticleMistUseFlatMistStartColor { get; private set; }
  public static ConfigEntry<float> ParticleMistDistantEmissionMaxVelocity { get; private set; }

  public static ConfigEntry<float> DemisterForceFieldGravity { get; private set; }
  public static ConfigEntry<bool> DemisterTriggerFadeOutParticleMist { get; private set; }
  public static ConfigEntry<float> DemisterTriggerFadeOutMultiplier { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            defaultValue: true,
            "Globally enable or disable this mod.");

    IsModEnabled.OnSettingChanged(ParticleMistManager.UpdateParticleMistSettings);
    IsModEnabled.OnSettingChanged(ParticleMistManager.UpdateDemisterSettings);

    ParticleMistReduceMotion =
        config.BindInOrder(
            "ParticleMist",
            "particleMistReduceMotion",
            defaultValue: true,
            "ParticleMist: reduces motion of the ParticleMist by setting Velocity/Rotation multipliers to 0.");

    ParticleMistReduceMotion.OnSettingChanged(ParticleMistManager.UpdateParticleMistSettings);

    ParticleMistUseFlatMistStartColor =
        config.BindInOrder(
            "ParticleMist",
            "particleMistUseFlatMistStartColor",
            defaultValue: false,
            "ParticleMist: uses only a single color for ParticleMist.m_ps.startColor");

    ParticleMistUseFlatMistStartColor.OnSettingChanged(ParticleMistManager.UpdateParticleMistSettings);

    ParticleMistDistantEmissionMaxVelocity =
        config.BindInOrder(
            "ParticleMist",
            "particleMistDistantEmissionMaxVelocity",
            defaultValue: 2f,
            "ParticleMist: override the setting for ParticleMist.m_distantEmissionMaxVel",
            new AcceptableValueRange<float>(0f, 2f));

    ParticleMistDistantEmissionMaxVelocity.OnSettingChanged(ParticleMistManager.UpdateParticleMistSettings);

    DemisterForceFieldGravity =
        config.BindInOrder(
            "Demister",
            "demisterForceFieldGravity",
            defaultValue: -5f,
            "Demister.m_forceField.gravity (vanilla: -0.08)",
            new AcceptableValueRange<float>(-10f, 0f));

    DemisterForceFieldGravity.OnSettingChanged(ParticleMistManager.UpdateDemisterSettings);

    DemisterTriggerFadeOutParticleMist =
        config.BindInOrder(
            "Demister",
            "demisterTriggerFadeOutParticleMist",
            defaultValue: true,
            "Demister: add a sphere collider trigger that fades-out ParticleMist inside.");

    DemisterTriggerFadeOutMultiplier =
        config.BindInOrder(
            "Demister",
            "demisterTriggerFadeOutMultiplier",
            defaultValue: 0.85f,
            "Demister: fade-out multiplier for the above trigger.",
            new AcceptableValueRange<float>(0f, 1f));

    DemisterTriggerFadeOutParticleMist.OnSettingChanged(ParticleMistManager.UpdateParticleMistSettings);
    DemisterTriggerFadeOutParticleMist.OnSettingChanged(ParticleMistManager.UpdateDemisterSettings);
  }
}
