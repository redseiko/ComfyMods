namespace Keysential;

using System.Collections.Generic;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ZoneSystem))]
static class ZoneSystemPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZoneSystem.Load))]
  static void LoadPostfix(ZoneSystem __instance) {
    if (!ZNet.m_isServer) {
      return;
    }

    Keysential.LogInfo($"Saved ZoneSystem.m_globalKeys are:\n{LogGlobalKeys(__instance)}");

    List<string> globalKeysOverrideList = GlobalKeysOverrideList.GetCachedStringList();

    if (globalKeysOverrideList.Count > 0) {
      __instance.m_globalKeys.Clear();
      __instance.m_globalKeys.UnionWith(globalKeysOverrideList);

      Keysential.LogInfo($"Overriding ZoneSystem.m_globalKeys to:\n{LogGlobalKeys(__instance)}");
    }

    List<string> globalKeysAllowedList = GlobalKeysAllowedList.GetCachedStringList();

    if (globalKeysAllowedList.Count > 0) {
      __instance.m_globalKeys.IntersectWith(globalKeysAllowedList);
      Keysential.LogInfo($"Limiting ZoneSystem.globalKeys for allowed list to:\n{LogGlobalKeys(__instance)}");
    }

    if (VendorKeyManagerEnabled.Value) {
      Keysential.LogInfo($"Adding VendorKeyManager component to ZoneSystem...");
      __instance.gameObject.AddComponent<VendorKeyManager>();
    }

    KeyManagerUtils.RunStartUpKeyManagers();
  }

  static string LogGlobalKeys(ZoneSystem zoneSystem) {
    return string.Join(",", zoneSystem.m_globalKeys);
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZoneSystem.RPC_SetGlobalKey))]
  static bool RPC_SetGlobalKeyPrefix(ZoneSystem __instance, long sender, string name) {
    if (!ZNet.m_isServer || IsGlobalKeyAllowed(name)) {
      return true;
    }

    Keysential.LogInfo($"Ignoring GlobalKey '{name}' from sender: {sender}");
    return false;
  }

  static bool IsGlobalKeyAllowed(string globalKey) {
    return
        GlobalKeysOverrideList.GetCachedStringList().IsEmptyOrContains(globalKey)
        && GlobalKeysAllowedList.GetCachedStringList().IsEmptyOrContains(globalKey);
  }
}
