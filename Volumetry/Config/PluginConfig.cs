namespace Volumetry;

using System.Collections.Generic;

using BepInEx.Configuration;

using ComfyLib;

using static ComfyLib.ToggleSliderListConfigEntry;

public static class PluginConfig {
  public static ConfigFile CurrentConfig { get; private set; }
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<float> AmbientLoopVolumeMax { get; private set; }
  public static ConfigEntry<float> OceanLoopVolumeMax { get; private set; }
  public static ConfigEntry<float> WindLoopVolumeMax { get; private set; }

  public static ConfigEntry<float> SfxVolumeMistlandsThunder { get; private set; }

  public static ToggleSliderListConfigEntry SfxVolumeOverrides { get; private set; }

  public static void BindConfig(ConfigFile config) {
    CurrentConfig = config;

    IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

    AmbientLoopVolumeMax =
        config.BindInOrder(
            "Loop.AmbientLoop",
            "ambientLoopVolumeMax",
            1f,
            "AmbientLoop: volume maximum.",
            new AcceptableValueRange<float>(0f, 1f));

    OceanLoopVolumeMax =
        config.BindInOrder(
            "Loop.OceanLoop",
            "oceanLoopVolumeMax",
            1f,
            "OceanLoop: volume maximum.",
            new AcceptableValueRange<float>(0f, 1f));

    WindLoopVolumeMax =
        config.BindInOrder(
            "Loop.WindLoop",
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

    SfxVolumeMistlandsThunder =
        config.BindInOrder(
            "SFX.Volume",
            "mistlandsThunder",
            1f,
            "SFX volume for: sfx_mistlands_thunder",
            new AcceptableValueRange<float>(0f, 1f));
  }

  static IEnumerable<SearchOption> GetSfxHistory() {
    return VolumeController.SfxHistorySearchOptions;
  }
}
