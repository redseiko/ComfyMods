namespace StatusQuo;

using System.Collections.Generic;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Hud))]
static class HudPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.Awake))]
  static void AwakePostfix(Hud __instance) {
    if (IsModEnabled.Value) {
      HudUtils.SetupStatusEffectListRoot(__instance, toggleOn: true);
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Hud.UpdateStatusEffects))]
  static void UpdateStatusEffectsPrefix(Hud __instance, List<StatusEffect> statusEffects, ref bool __state) {
    __state = IsModEnabled.Value && __instance.m_statusEffects.Count != statusEffects.Count;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Hud.UpdateStatusEffects))]
  static void UpdateStatusEffectsPostfix(Hud __instance, bool __state) {
    if (__state) {
      HudUtils.SetupStatusEffects(__instance);
    }
  }
}
