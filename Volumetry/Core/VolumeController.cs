namespace Volumetry;

using System.Collections.Generic;

using static ComfyLib.ToggleSliderListConfigEntry;
using static PluginConfig;

public static class VolumeController {
  public static readonly HashSet<string> SFXHistory = [];
  public static readonly List<SearchOption> SfxHistorySearchOptions = [];

  // TODO: do this.
  // public static readonly Dictionary<string, float> SfxVolumeOverrideMap = [];

  public static void ProcessSFX(ZSFX sfx) {
    string sfxName = Utils.GetPrefabName(sfx.name);

    if (SFXHistory.Add(sfxName)) {
      SfxHistorySearchOptions.Insert(0, new(sfxName));
    }

    if (sfxName == "sfx_mistlands_thunder") {
      SetSFXVolume(sfx, SfxVolumeMistlandsThunder.Value);
    }
  }

  public static void SetSFXVolume(ZSFX sfx, float volume) {
    sfx.m_audioSource.volume = volume;
    sfx.m_vol = volume;
  }
}
