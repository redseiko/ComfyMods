namespace Volumetry;

using System.Collections.Generic;
using System.Linq;

using ComfyLib;

public static class VolumeController {
  public static readonly CircularQueue<SearchOption> SfxHistorySearchOptions = new(50);
  public static readonly Dictionary<string, float> SfxVolumeOverrideMap = [];

  public static void ProcessSfx(ZSFX sfx) {
    string sfxName = Utils.GetPrefabName(sfx.name);
    string clipName = sfx.m_audioSource.clip.name;

    if (SfxVolumeOverrideMap.TryGetValue(sfxName, out float volume)
        || SfxVolumeOverrideMap.TryGetValue(clipName, out volume)) {
      Volumetry.LogInfo($"Overring SFX {sfxName} ({clipName}) volume from {sfx.m_vol:F0} to {volume:F0}.");
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
}
