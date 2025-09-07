namespace Volumetry;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(AudioMan))]
static class AudioManPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(AudioMan.UpdateAmbientLoop))]
  static void UpdateAmbientLoopPostfix(AudioMan __instance) {
    if (IsModEnabled.Value) {
      float volume = __instance.m_ambientLoopSource.volume;
      __instance.m_ambientLoopSource.volume = Mathf.Min(volume, AmbientLoopVolumeMax.Value);
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(AudioMan.UpdateOceanAmbiance))]
  static void UpdateOceanAmbiencePostfix(AudioMan __instance) {
    if (IsModEnabled.Value) {
      float volume = __instance.m_oceanAmbientSource.volume;
      __instance.m_oceanAmbientSource.volume = Mathf.Min(volume, OceanLoopVolumeMax.Value);
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(AudioMan.UpdateWindAmbience))]
  static void UpdateWindAmbiencePostfix(AudioMan __instance) {
    if (IsModEnabled.Value) {
      float volume = __instance.m_windLoopSource.volume;
      __instance.m_windLoopSource.volume = Mathf.Min(volume, WindLoopVolumeMax.Value);
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(AudioMan.UpdateShieldHum))]
  static void UpdateShieldHumPostfix(AudioMan __instance) {
    if (IsModEnabled.Value && __instance.m_enableShieldDomeHum) {
      float volume = __instance.m_shieldHumSource.volume;
      __instance.m_shieldHumSource.volume = Mathf.Min(volume, ShieldHumVolumeMax.Value);
    }
  }
}
