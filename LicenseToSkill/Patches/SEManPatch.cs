using HarmonyLib;

using static LicenseToSkill.LicenseToSkill;
using static LicenseToSkill.PluginConfig;

namespace LicenseToSkill {
  [HarmonyPatch(typeof(SEMan))]
  static class SEManPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(SEMan.AddStatusEffect), typeof(int), typeof(bool), typeof(int), typeof(float))]
    static void AddStatusEffectPostfix(
        ref SEMan __instance, ref StatusEffect __result, int nameHash, bool resetTime) {
      if (!IsModEnabled.Value
          || !__result
          || __instance.m_character != Player.m_localPlayer
          || nameHash != Player.s_statusEffectSoftDeath) {
        return;
      }

      __result.m_ttl = GetConfigHardDeathCooldown();
    }
  }
}
