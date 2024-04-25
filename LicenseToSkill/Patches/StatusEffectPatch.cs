namespace LicenseToSkill;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(StatusEffect))]
static class StatusEffectPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(StatusEffect.GetIconText))]
  static bool GetIconTextPrefix(StatusEffect __instance, ref string __result) {
    if (IsModEnabled.Value || (__instance.NameHash() != SEMan.s_statusEffectSoftDeath)) {
      return true;
    }

    __result = string.Empty;
    return false;
  }
}
