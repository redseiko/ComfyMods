namespace SkyTree;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(ZoneSystem))]
static class ZoneSystemPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZoneSystem.SpawnZone))]
  static void SpawnZonePrefix() {
    if (IsModEnabled.Value) {
      YggdrasilManager.SetYggdrasilLayer(YggdrasilManager.SkyboxLayer);
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZoneSystem.SpawnZone))]
  static void SpawnZonePostfix() {
    if (IsModEnabled.Value) {
      YggdrasilManager.SetYggdrasilLayer(YggdrasilManager.StaticSolidLayer);
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZoneSystem.IsBlocked))]
  static bool IsBlockedPrefix(ZoneSystem __instance, Vector3 p, ref bool __result) {
    if (IsModEnabled.Value) {
      __result = YggdrasilManager.IsBlockedDelegate(__instance.m_blockRayMask, p);
      return false;
    }

    return true;
  }
}
