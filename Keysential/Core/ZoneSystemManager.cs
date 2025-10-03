namespace Keysential;

using System.Collections.Generic;

using ComfyLib;

using static PluginConfig;

public static class ZoneSystemManager {
  public static void SetupGlobalKeys(ZoneSystem zoneSystem) {
    Keysential.LogInfo($"Saved ZoneSystem.m_globalKeys are:\n{LogGlobalKeys(zoneSystem)}");

    HashSet<string> globalKeysOverrideSet = GlobalKeysOverrideList.GetCachedStringSet();

    if (globalKeysOverrideSet.Count > 0) {
      zoneSystem.m_globalKeys.Clear();
      zoneSystem.m_globalKeys.UnionWith(globalKeysOverrideSet);

      Keysential.LogInfo($"Overriding ZoneSystem.m_globalKeys to:\n{LogGlobalKeys(zoneSystem)}");
    }

    HashSet<string> globalKeysAllowedSet = GlobalKeysAllowedList.GetCachedStringSet();

    if (globalKeysAllowedSet.Count > 0) {
      zoneSystem.m_globalKeys.IntersectWith(globalKeysAllowedSet);
      Keysential.LogInfo($"Limiting ZoneSystem.globalKeys for allowed set to:\n{LogGlobalKeys(zoneSystem)}");
    }

    if (VendorKeyManagerEnabled.Value) {
      Keysential.LogInfo($"Adding VendorKeyManager component to ZoneSystem...");
      zoneSystem.gameObject.AddComponent<VendorKeyManager>();
    }
  }

  static string LogGlobalKeys(ZoneSystem zoneSystem) {
    return string.Join(",", zoneSystem.m_globalKeys);
  }

  public static bool ShouldIgnoreSetGlobalKey(ZoneSystem zoneSystem, long senderId, string globalKey) {
    if (IsGlobalKeyAllowed(globalKey)) {
      return false;
    }

    Keysential.LogInfo($"Ignoring SetGlobalKey '{globalKey}' from senderId {senderId}.");

    return true;
  }

  static bool IsGlobalKeyAllowed(string globalKey) {
    return
        GlobalKeysOverrideList.GetCachedStringSet().IsEmptyOrContains(globalKey)
        && GlobalKeysAllowedList.GetCachedStringSet().IsEmptyOrContains(globalKey);
  }
}
