namespace Volumetry;

using System.Collections.Generic;

using ComfyLib;

public static class VolumeController {
  public static readonly CircularQueue<SearchOption> SfxHistorySearchOptions = new(50);
  public static readonly Dictionary<string, float> SfxVolumeOverrideMap = [];

  public static void ProcessSfx(ZSFX sfx) {
    string sfxName = Utils.GetPrefabName(sfx.name);

    if (SfxVolumeOverrideMap.TryGetValue(sfxName, out float volume)) {
      Volumetry.LogInfo($"Overring SFX {sfxName} volume from {sfx.m_vol:F0} to {volume:F0}.");
      SetSfxVolume(sfx, volume);
    }

    AddSfxToHistory(sfx, sfxName);
  }

  static readonly HashSet<string> _sfxHistoryCache = [];

  static void AddSfxToHistory(ZSFX sfx, string sfxName) {
    if (_sfxHistoryCache.Add(sfxName)) {
      SfxHistorySearchOptions.Enqueue(new(sfxName));
    }
  }

  public static void SetSfxVolume(ZSFX sfx, float volume) {
    sfx.m_audioSource.volume = volume;
    sfx.m_vol = volume;
  }
}
