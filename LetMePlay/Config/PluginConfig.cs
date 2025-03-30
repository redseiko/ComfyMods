namespace LetMePlay;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<bool> DisableWardShieldFlash { get; private set; }
  public static ConfigEntry<bool> DisableBuildPlacementMarker { get; private set; }

  public static ConfigEntry<bool> DisableWeatherSnowParticles { get; private set; }
  public static ConfigEntry<bool> DisableWeatherAshParticles { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    IsModEnabled.OnSettingChanged(EnvManagerUtils.SetupCurrentWeather);

    DisableWardShieldFlash =
        config.BindInOrder(
            "Effects",
            "disableWardShieldFlash",
            false,
            "Disable wards from flashing their blue shield.");

    DisableBuildPlacementMarker =
        config.BindInOrder(
            "Build",
            "disableBuildPlacementMarker",
            false,
            "Disables the yellow placement marker (and gizmo indicator) when building.");

    DisableWeatherSnowParticles =
        config.BindInOrder(
            "Weather",
            "disableWeatherSnowParticles",
            false,
            "Disables ALL snow particles during snow/snowstorm weather.");

    DisableWeatherSnowParticles.OnSettingChanged(EnvManagerUtils.SetupCurrentWeather);

    DisableWeatherAshParticles =
        config.BindInOrder(
            "Weather",
            "disableWeatherAshParticles",
            false,
            "Disables ALL ash particles during ash rain weather.");

    DisableWeatherAshParticles.OnSettingChanged(EnvManagerUtils.SetupCurrentWeather);
  }
}
