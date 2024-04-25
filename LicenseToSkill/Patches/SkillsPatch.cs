namespace LicenseToSkill;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Skills))]
static class SkillsPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(Skills.LowerAllSkills))]
  static void LowerAllSkillsPrefix(Skills __instance, ref float factor) {
    if (IsModEnabled.Value && __instance.m_player == Player.m_localPlayer) {
      factor = SkillLossPercentOverride.Value * 0.01f;
    }
  }
}
