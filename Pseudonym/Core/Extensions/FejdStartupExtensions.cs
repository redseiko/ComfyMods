namespace Pseudonym;

public static class FejdStartupExtensions {
  public static bool TryGetPlayerProfile(this FejdStartup fejdStartup, out PlayerProfile profile) {
    if (fejdStartup) {
      return TryGetPlayerProfile(fejdStartup, out profile, fejdStartup.m_profileIndex);
    }

    profile = default;
    return false;
  }

  public static bool TryGetPlayerProfile(this FejdStartup fejdStartup, out PlayerProfile profile, int profileIndex) {
    profile = default;

    if (!fejdStartup
        || fejdStartup.m_profiles == null
        || fejdStartup.m_profiles.Count <= 0) {
      return false;
    }

    if (profileIndex < 0 || profileIndex >= fejdStartup.m_profiles.Count) {
      return false;
    }

    profile = fejdStartup.m_profiles[profileIndex];
    return true;
  }
}
