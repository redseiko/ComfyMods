namespace LicenseToSkill;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Skills))]
static class SkillsPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(Skills.LowerAllSkills))]
  static void LowerAllSkillsPrefix(Skills __instance, ref float factor) {
    if (IsModEnabled.Value && __instance.m_player == Player.m_localPlayer) {
      factor = Mathf.Min(SkillLossPercentOverride.Value * 0.01f, factor);
    }
  }
}
