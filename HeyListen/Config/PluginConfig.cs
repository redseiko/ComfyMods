namespace HeyListen;

using BepInEx.Configuration;

using ComfyLib;

using System;

using UnityEngine;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<bool> DemisterBallLockPosition { get; private set; }
  public static ConfigEntry<Vector3> DemisterBallLockOffset { get; private set; }
  public static ConfigEntry<bool> DemisterBallUseCustomSettings { get; private set; }

  public static ConfigEntry<float> DemisterBallBodyScale { get; private set; }
  public static ConfigEntry<Color> DemisterBallBodyColor { get; private set; }
  public static ConfigEntry<float> DemisterBallBodyBrightness { get; private set; }
  public static ConfigEntry<Color> DemisterBallPointLightColor { get; private set; }

  [Flags]
  public enum FlameEffects {
    None = 0,
    Flames = 1,
    FlamesL = 2,
    Flare = 4,
    Embers = 8,
    Distortion = 16,
    Energy = 32,
    EnergyII = 64,
    Sparcs = 128
  }

  public static readonly FlameEffects DefaultFlameEffects =
      FlameEffects.Flare | FlameEffects.Embers | FlameEffects.EnergyII | FlameEffects.Sparcs;

  public static ConfigEntry<FlameEffects> DemisterBallFlameEffectsEnabled { get; private set; }
  public static ConfigEntry<Color> DemisterBallFlameEffectsColor { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    IsModEnabled.OnSettingChanged(WispManager.UpdateUseCustomSettings);

    DemisterBallLockPosition =
        config.BindInOrder(
            "DemisterBall",
            "demisterBallLockPosition",
            true,
            "SE_Demister.m_ballPrefab.transform.position lock to player.");

    DemisterBallLockOffset =
        config.BindInOrder(
            "DemisterBall",
            "demisterBallLockOffset",
            new Vector3(-0.2f, 0.5f, 0f),
            "SE_Demister.m_ballPrefab.transform.position offset when locked to player.");

    DemisterBallUseCustomSettings =
        config.BindInOrder(
            "DemisterBall.Customization",
            "demisterBallUseCustomSettings",
            true,
            "Enables DemisterBall (Wisp) customization.");

    DemisterBallUseCustomSettings.OnSettingChanged(WispManager.UpdateUseCustomSettings);

    DemisterBallBodyScale =
        config.BindInOrder(
            "Effects.Body",
            "bodyScale",
            1f,
            "SE_demister.m_ballPrefab.transform.localScale",
            new AcceptableValueRange<float>(0f, 2f));

    DemisterBallBodyScale.OnSettingChanged(WispManager.UpdatePlayerDemisterBall);

    DemisterBallBodyColor =
        config.BindInOrder(
            "Effects.Body",
            "bodyColor",
            new Color(0f, 0.832f, 1f, 1f),
            "SE Demister.m_ballPrefab.color");

    DemisterBallBodyColor.OnSettingChanged(WispManager.UpdatePlayerDemisterBall);

    DemisterBallBodyBrightness =
        config.BindInOrder(
            "Effects.Body",
            "bodyBrightness",
            1.2f,
            "SE_Demister.m_ballPrefab.brightness",
            new AcceptableValueRange<float>(0f, 2f));

    DemisterBallBodyBrightness.OnSettingChanged(WispManager.UpdatePlayerDemisterBall);

    DemisterBallPointLightColor =
        config.BindInOrder(
            "Effects.PointLight",
            "pointLightColor",
            new Color(0.482f, 0.803f, 1f, 1f),
            "SE_Demister.m_ballPrefab/effects/Point light.color");

    DemisterBallPointLightColor.OnSettingChanged(WispManager.UpdatePlayerDemisterBall);

    DemisterBallFlameEffectsEnabled =
        config.BindInOrder(
            "FlameEffects",
            "flameEffectsEnabled",
            DefaultFlameEffects,
            "SE_Demister.m_ballPrefab/effects/flame/...");

    DemisterBallFlameEffectsEnabled.OnSettingChanged(WispManager.UpdatePlayerDemisterBallFlameEffects);

    DemisterBallFlameEffectsColor =
        config.BindInOrder(
            "FlameEffects",
            "flameEffectsColor",
            new Color(0.482f, 0.803f, 1f, 1f),
            "SE_Demister.m_ballPrefab/effects/flame/... color.");

    DemisterBallFlameEffectsColor.OnSettingChanged(WispManager.UpdatePlayerDemisterBallFlameEffects);

    BindFlameEffectsEmbersConfig(config);
    BindFlameEffectsSparcsConfig(config);
  }

  public static ConfigEntry<Color> FlameEffectsEmbersColor { get; private set; }
  public static ConfigEntry<float> FlameEffectsEmbersBrightness { get; private set; }

  public static void BindFlameEffectsEmbersConfig(ConfigFile config) {
    FlameEffectsEmbersColor =
        config.BindInOrder(
            "FlameEffects.Embers",
            "embersColor",
            new Color(0f, 0.677f, 1f, 1f),
            "m_ballPrefab/effects/flame/embers.color");

    FlameEffectsEmbersColor.OnSettingChanged(WispManager.UpdatePlayerDemisterBallFlameEffects);

    FlameEffectsEmbersBrightness =
        config.BindInOrder(
            "FlameEffects.Embers",
            "embersBrightness",
            defaultValue: 5.34f,
            "m_ballPrefab/effects/flame/embers.brightness",
            new AcceptableValueRange<float>(0f, 5.34f));

    FlameEffectsEmbersBrightness.OnSettingChanged(WispManager.UpdatePlayerDemisterBallFlameEffects);
  }

  public static ConfigEntry<Color> FlameEffectsSparcsColor { get; private set; }
  public static ConfigEntry<float> FlameEffectsSparcsBrightness { get; private set; }

  public static void BindFlameEffectsSparcsConfig(ConfigFile config) {
    FlameEffectsSparcsColor =
      config.BindInOrder(
          "FlameEffects.Sparcs",
          "sparcsColor",
          new Color(0f, 0.677f, 1f, 1f),
          "m_ballPrefab/effects/flame/sparcs.color");

    FlameEffectsSparcsColor.OnSettingChanged(WispManager.UpdatePlayerDemisterBallFlameEffects);

    FlameEffectsSparcsBrightness =
        config.BindInOrder(
            "FlameEffects.Sparcs",
            "sparcsBrightness",
            defaultValue: 5.34f,
            "m_ballPrefab/effects/flame/sparcs.brightness",
            new AcceptableValueRange<float>(0f, 5.34f));

    FlameEffectsSparcsBrightness.OnSettingChanged(WispManager.UpdatePlayerDemisterBallFlameEffects);
  }
}
