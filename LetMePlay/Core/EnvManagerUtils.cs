namespace LetMePlay;

using static PluginConfig;

public static class EnvManagerUtils {
  public static bool IsSnowWeather(string weatherName) {
    switch (weatherName) {
      case "Snow":
      case "SnowStorm":
      case "Twilight_Snow":
      case "Twilight_SnowStorm":
        return true;

      default:
        return false;
    }
  }

  public static bool IsAshWeather(string weatherName) {
    switch (weatherName) {
      case "Ashlands_ashrain":
      case "Ashlands_storm":
        return true;

      default:
        return false;
    }
  }

  public static void SetupCurrentWeather() {
    EnvMan envManager = EnvMan.s_instance;

    if (!envManager || envManager.m_currentEnv == default  || envManager.m_currentPSystems == default) {
      return;
    }

    if (IsSnowWeather(envManager.m_currentEnv.m_name)) {
      envManager.SetParticleArrayEnabled(
          envManager.m_currentPSystems, !(IsModEnabled.Value && DisableWeatherSnowParticles.Value));
    }

    if (IsAshWeather(envManager.m_currentEnv.m_name)) {
      envManager.SetParticleArrayEnabled(
          envManager.m_currentPSystems, !(IsModEnabled.Value && DisableWeatherAshParticles.Value));
    }
  }
}
