namespace Dramamist;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ParticleMist))]
static class ParticleMistPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ParticleMist.Awake))]
  static void Awake(ParticleMist __instance) {
    if (IsModEnabled.Value) {
      ParticleMistManager.UpdateParticleMistSettings();
    }

    __instance.m_ps.gameObject.AddComponent<ParticleMistTriggerCallback>();
  }
}
