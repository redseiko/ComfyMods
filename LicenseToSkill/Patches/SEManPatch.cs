namespace LicenseToSkill;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(SEMan))]
static class SEManPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(SEMan.AddStatusEffect), typeof(int), typeof(bool), typeof(int), typeof(float))]
  static void AddStatusEffectPostfix(SEMan __instance, ref StatusEffect __result) {
    if (IsModEnabled.Value
        && Player.m_localPlayer == __instance.m_character
        && __result
        && __result.NameHash() == SEMan.s_statusEffectSoftDeath) {
      __result.m_ttl = StatusEffectUtils.GetConfigHardDeathCooldown();
    }
  }
}
