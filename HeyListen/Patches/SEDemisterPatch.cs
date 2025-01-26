namespace HeyListen;

using HarmonyLib;

using UnityEngine;
using UnityEngine.Animations;

using static PluginConfig;

[HarmonyPatch(typeof(SE_Demister))]
static class SEDemisterPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(SE_Demister.Setup))]
  static void SetupPostfix(SE_Demister __instance, Character character) {
    if (IsModEnabled.Value && character == Player.m_localPlayer) {
      __instance.m_noiseSpeed *= Random.Range(0.8f, 1.2f);
      __instance.m_noiseDistance *= Random.Range(0.8f, 1.2f);
      __instance.m_noiseDistanceInterior *= Random.Range(0.8f, 1.2f);
      __instance.m_noiseDistanceYScale *= Random.Range(0.8f, 1.2f);
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(SE_Demister.UpdateStatusEffect))]
  static void UpdateStatusEffectPrefix(SE_Demister __instance, ref bool __state) {
    if (IsModEnabled.Value) {
      __state = __instance.m_ballInstance;
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(SE_Demister.UpdateStatusEffect))]
  static void UpdateStatusEffectPostfix(SE_Demister __instance, ref bool __state) {
    if (!IsModEnabled.Value || !__instance.m_ballInstance) {
      return;
    }

    if ((!__state || !WispManager.LocalPlayerDemisterBall) && __instance.m_character == Player.m_localPlayer) {
      WispManager.LocalPlayerDemister = __instance;

      if (__instance.m_ballInstance.TryGetComponent(out DemisterBallControl demisterBallControl)) {
        WispManager.SetLocalPlayerDemisterBallControl(demisterBallControl);
      }

      if (!__instance.m_ballInstance.TryGetComponent(out ParentConstraint _)) {
        WispManager.SetupParentConstraint(
            __instance.m_ballInstance.AddComponent<ParentConstraint>(), __instance.m_character);
      }
    }
  }
}
