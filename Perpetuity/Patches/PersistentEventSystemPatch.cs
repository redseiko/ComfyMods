namespace Perpetuity;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(PersistentEventSystem))]
static class PersistentEventSystemPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(PersistentEventSystem.Load))]
  static void LoadPostfix(PersistentEventSystem __instance) {
    if (!IsPersistentEventSystemEnabled.Value) {
      __instance.m_activePersistentEvents.list.Clear();
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(PersistentEventSystem.RPC_RequestStartEvent))]
  static bool RPC_RequestStartEventPrefix(PersistentEventSystem __instance, long sender, int sourceEventId) {
    if (IsPersistentEventSystemEnabled.Value) {
      return true;
    }

    return false;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(PersistentEventSystem.Start))]
  static void StartPostfix(PersistentEventSystem __instance) {
    if (ApplyPersistentEventSystemOverrides.Value) {
      PersistentEventManager.ApplyOverrides(__instance);
    }
  }
}
