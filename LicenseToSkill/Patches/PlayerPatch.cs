using HarmonyLib;

using static LicenseToSkill.LicenseToSkill;
using static LicenseToSkill.PluginConfig;

namespace LicenseToSkill {
  [HarmonyPatch(typeof(Player))]
  static class PlayerPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Player.OnSpawned))]
    static void SetupAwakePostfix(ref Player __instance) {
      if (!IsModEnabled.Value || __instance != Player.m_localPlayer) {
        return;
      }

      __instance.m_hardDeathCooldown = GetConfigHardDeathCooldown();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Player.HardDeath))]
    static void HardDeathPostfix(ref Player __instance, ref bool __result) {
      if (!IsModEnabled.Value || __instance != Player.m_localPlayer) {
        return;
      }

      __result = __instance.m_timeSinceDeath > GetConfigHardDeathCooldown();
    }
  }
}
