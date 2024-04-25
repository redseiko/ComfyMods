namespace LicenseToSkill;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Player))]
static class PlayerPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.OnSpawned))]
  static void OnSpawnedPostfix(ref Player __instance) {
    if (IsModEnabled.Value && __instance == Player.m_localPlayer) {
      __instance.m_hardDeathCooldown = StatusEffectUtils.GetConfigHardDeathCooldown();
    }   
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Player.HardDeath))]
  static void HardDeathPostfix(Player __instance, ref bool __result) {
    if (IsModEnabled.Value && __instance == Player.m_localPlayer) {
      __result = __instance.m_timeSinceDeath > StatusEffectUtils.GetConfigHardDeathCooldown();
    }   
  }
}
