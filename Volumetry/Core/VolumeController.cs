namespace Volumetry;

using static PluginConfig;

public static class VolumeController {
  public static void ProcessSFX(ZSFX sfx) {
    if (sfx.name.StartsWith("sfx_mistlands_thunder", System.StringComparison.Ordinal)) {
      sfx.m_audioSource.volume = SfxVolumeMistlandsThunder.Value;
      sfx.m_vol = SfxVolumeMistlandsThunder.Value;
    }
  }
}
