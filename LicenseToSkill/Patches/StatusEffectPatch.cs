using HarmonyLib;

using static LicenseToSkill.PluginConfig;

namespace LicenseToSkill.Patches {
  [HarmonyPatch(typeof(StatusEffect))]
  static class StatusEffectPatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(StatusEffect.GetIconText))]
    public static bool GetIconTextPrefix(ref StatusEffect __instance, ref string __result) {
      if (IsModEnabled.Value
          || __instance.NameHash() != Player.s_statusEffectSoftDeath) {

        return true;
      }

      __result = "";
      return false;
    }
  }
}
