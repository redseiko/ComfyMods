namespace Volumetry;

using System.Collections.Generic;
using System.Linq;

using ComfyLib;

using UnityEngine;

public static class VolumeController {
  public static readonly CircularQueue<SearchOption> SfxHistorySearchOptions = new(50);
  public static readonly Dictionary<string, float> SfxVolumeOverrideMap = [];

  public static void ProcessSfx(ZSFX sfx) {
    string sfxName = Utils.GetPrefabName(sfx.name);
    string clipName = sfx.m_audioSource.clip.name;

    if (SfxVolumeOverrideMap.TryGetValue(sfxName, out float volume)
        || SfxVolumeOverrideMap.TryGetValue(clipName, out volume)) {
      Volumetry.LogInfo($"Overriding SFX {sfxName} ({clipName}) volume from {sfx.m_vol:F0} to {volume:F0}.");
      SetSfxVolume(sfx, volume);
    }

    AddSfxToHistory(sfxName, clipName);
  }

  public static void ProcessCurrentSfx() {
    foreach (ZSFX sfx in ZSFX.Instances.Cast<ZSFX>()) {
      if (sfx.m_audioSource
          && sfx.m_audioSource.clip
          && (SfxVolumeOverrideMap.TryGetValue(Utils.GetPrefabName(sfx.name), out float volume)
              || SfxVolumeOverrideMap.TryGetValue(sfx.m_audioSource.clip.name, out volume))) {
        SetSfxVolume(sfx, volume);
      }
    }
  }

  static readonly HashSet<string> _sfxHistoryCache = [];

  static void AddSfxToHistory(string sfxName, string clipName) {
    string displayName = $"{sfxName}\n{clipName}";

    if (_sfxHistoryCache.Add(displayName)) {
      SfxHistorySearchOptions.Enqueue(new(sfxName, clipName));
    }
  }

  public static void SetSfxVolume(ZSFX sfx, float volume) {
    sfx.m_audioSource.volume = volume;
    sfx.m_vol = volume;
  }

  public static readonly CircularQueue<SearchOption> EffectFadeHistorySearchOptions = new(50);
  public static readonly Dictionary<string, float> EffectFadeVolumeOverrideMap = [];

  static bool TryGetEffectFadeVolumeOverride(EffectFade effectFade, out float volume) {
    return
        EffectFadeVolumeOverrideMap.TryGetValue(Utils.GetPrefabName(effectFade.transform.root.name), out volume)
        || EffectFadeVolumeOverrideMap.TryGetValue(effectFade.m_audioSource.clip.name, out volume);
  }

  public static void ProcessEffectFade(EffectFade effectFade) {
    string prefabName = Utils.GetPrefabName(effectFade.transform.root.name);
    string clipName = effectFade.m_audioSource.clip.name;

    if (EffectFadeVolumeOverrideMap.TryGetValue(prefabName, out float volume)
        || EffectFadeVolumeOverrideMap.TryGetValue(clipName, out volume)) {
      Volumetry.LogInfo(
          $"Overriding EffectFade {prefabName} ({clipName}) volume from {effectFade.m_baseVolume:F0} to {volume:F0}.");
      SetEffectFadeVolume(effectFade, volume);
    }

    AddEffectFadeToHistory(prefabName, clipName);
  }

  public static void ProcessCurrentEffectFade() {
    EffectFade[] effectFades =
        Object.FindObjectsByType<EffectFade>(FindObjectsInactive.Include, FindObjectsSortMode.None);

    foreach (EffectFade effectFade in effectFades) {
      if (effectFade.m_audioSource
          && effectFade.m_audioSource.clip
          && TryGetEffectFadeVolumeOverride(effectFade, out float volume)) {
        SetEffectFadeVolume(effectFade, volume);
      }
    }
  }

  static readonly HashSet<string> _effectFadeHistoryCache = [];

  static void AddEffectFadeToHistory(string prefabName, string clipName) {
    string displayName = $"{prefabName}\n{clipName}";

    if (_effectFadeHistoryCache.Add(displayName)) {
      EffectFadeHistorySearchOptions.Enqueue(new(prefabName, clipName));
    }
  }

  public static void SetEffectFadeVolume(EffectFade effectFade, float volume) {
    effectFade.m_baseVolume = volume;
  }
}
