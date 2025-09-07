namespace Volumetry;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

public static class PluginConfig {
  public static ConfigFile CurrentConfig { get; private set; }
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<float> AmbientLoopVolumeMax { get; private set; }
  public static ConfigEntry<float> OceanLoopVolumeMax { get; private set; }
  public static ConfigEntry<float> ShieldHumVolumeMax { get; private set; }
  public static ConfigEntry<float> WindLoopVolumeMax { get; private set; }

  public static VolumeSliderListConfigEntry SfxVolumeOverrides { get; private set; }
  public static VolumeSliderListConfigEntry EffectFadeVolumeOverrides { get; private set; }

  public static void BindConfig(ConfigFile config) {
    CurrentConfig = config;

    IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

    AmbientLoopVolumeMax =
        config.BindInOrder(
            "Loop",
            "ambientLoopVolumeMax",
            1f,
            "AmbientLoop: volume maximum.",
            new AcceptableValueRange<float>(0f, 1f));

    OceanLoopVolumeMax =
        config.BindInOrder(
            "Loop",
            "oceanLoopVolumeMax",
            1f,
            "OceanLoop: volume maximum.",
            new AcceptableValueRange<float>(0f, 1f));

    ShieldHumVolumeMax =
        config.BindInOrder(
            "Loop",
            "shieldHumVolumeMax",
            1f,
            "ShieldHum: volume maximum.",
            new AcceptableValueRange<float>(0f, 1f));

    WindLoopVolumeMax =
        config.BindInOrder(
            "Loop",
            "windLoopVolumeMax",
            1f,
            "WindLoop: volume maximum.",
            new AcceptableValueRange<float>(0f, 1f));

    SfxVolumeOverrides =
        new(
            config,
            "SFX.Volume",
            "sfxVolumeOverrides",
            string.Empty,
            "SFX volume overrides.",
            GetSfxHistory);

    SfxVolumeOverrides.ConfigEntry.OnSettingChanged(OnSfxVolumeOverridesChanged);

    EffectFadeVolumeOverrides =
        new(
            config,
            "EffectFade.Volume",
            "effectFadeVolumeOverrides",
            string.Empty,
            "EffectFade volume overrides.",
            GetEffectFadeHistory);

    EffectFadeVolumeOverrides.ConfigEntry.OnSettingChanged(OnEffectFadeVolumeOverridesChanged);
  }

  static IEnumerable<SearchOption> GetSfxHistory() {
    return VolumeController.SfxHistorySearchOptions.Reverse();
  }

  // TODO: encapsulate all of this into a reuseable class like DelayableOnSettingChangedSomething.

  static void OnSfxVolumeOverridesChanged() {
    _sfxVolumeChangedCoroutineEnd = Time.time + 0.35f;

    if (_sfxVolumeChangedCoroutine == default) {
      _sfxVolumeChangedCoroutine = MonoUpdaters.s_instance.StartCoroutine(OnSfxVolumeOverridesChangedCoroutine());
    }
  }

  static Coroutine _sfxVolumeChangedCoroutine = default;
  static float _sfxVolumeChangedCoroutineEnd = 0f;

  static IEnumerator OnSfxVolumeOverridesChangedCoroutine() {
    while (Time.time < _sfxVolumeChangedCoroutineEnd) {
      yield return null;
    }

    VolumeController.SfxVolumeOverrideMap.Clear();

    foreach ((string name, float volume) in SfxVolumeOverrides.GetToggledValues()) {
      VolumeController.SfxVolumeOverrideMap[name] = volume;
    }

    Volumetry.LogInfo($"SFX volume overrides: {VolumeController.SfxVolumeOverrideMap.Count}");
    VolumeController.ProcessCurrentSfx();

    _sfxVolumeChangedCoroutine = default;
  }

  static IEnumerable<SearchOption> GetEffectFadeHistory() {
    return VolumeController.EffectFadeHistorySearchOptions.Reverse();
  }

  static void OnEffectFadeVolumeOverridesChanged() {
    _effectFadeChangedCoroutineEnd = Time.time + 0.35f;

    if (_effectFadeChangedCoroutine == default) {
      _effectFadeChangedCoroutine =
          MonoUpdaters.s_instance.StartCoroutine(OnEffectFadeVolumeOverridesChangedCoroutine());
    }
  }

  static Coroutine _effectFadeChangedCoroutine = default;
  static float _effectFadeChangedCoroutineEnd = 0f;

  static IEnumerator OnEffectFadeVolumeOverridesChangedCoroutine() {
    while (Time.time < _effectFadeChangedCoroutineEnd) {
      yield return null;
    }

    VolumeController.EffectFadeVolumeOverrideMap.Clear();

    foreach ((string name, float volume) in EffectFadeVolumeOverrides.GetToggledValues()) {
      VolumeController.EffectFadeVolumeOverrideMap[name] = volume;
    }

    Volumetry.LogInfo($"EffectFade volume overrides: {VolumeController.EffectFadeVolumeOverrideMap.Count}");
    VolumeController.ProcessCurrentEffectFade();

    _effectFadeChangedCoroutine = default;
  }
}
