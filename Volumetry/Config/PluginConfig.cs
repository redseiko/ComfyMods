namespace Volumetry;

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
  public static ConfigEntry<float> WindLoopVolumeMax { get; private set; }

  public static ToggleSliderListConfigEntry SfxVolumeOverrides { get; private set; }

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
            "sfx_mistlands_thunder;0;1", // Set to string.Empty after.
            "SFX volume overrides.",
            GetSfxHistory);

    SfxVolumeOverrides.ConfigEntry.OnSettingChanged(OnSfxVolumeOverridesChanged);
  }

  static IEnumerable<SearchOption> GetSfxHistory() {
    return VolumeController.SfxHistorySearchOptions.Reverse();
  }

  // TODO: encapsulate all of this into a reuseable class like DelayableOnSettingChangedSomething.

  static void OnSfxVolumeOverridesChanged() {
    _changedCoroutineEnd = Time.time + 0.5f;

    if (_changedCoroutine == default) {
      _changedCoroutine = MonoUpdaters.s_instance.StartCoroutine(OnSfxVolumeOverridesChangedCoroutine());
    }
  }

  static Coroutine _changedCoroutine = default;
  static float _changedCoroutineEnd = 0f;

  static IEnumerator OnSfxVolumeOverridesChangedCoroutine() {
    while (Time.time < _changedCoroutineEnd) {
      yield return null;
    }

    VolumeController.SfxVolumeOverrideMap.Clear();

    foreach ((string name, float volume) in SfxVolumeOverrides.GetToggledValues()) {
      VolumeController.SfxVolumeOverrideMap[name] = volume;
    }

    Volumetry.LogInfo($"SFX volume overrides: {VolumeController.SfxVolumeOverrideMap.Count}");
    VolumeController.ProcessCurrentSfx();

    _changedCoroutine = default;
  }
}
