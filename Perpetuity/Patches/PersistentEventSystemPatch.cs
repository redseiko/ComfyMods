namespace Perpetuity;

using HarmonyLib;

[HarmonyPatch(typeof(PersistentEventSystem))]
static class PersistentEventSystemPatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(PersistentEventSystem.RPC_RequestStartEvent))]
  static bool RPC_RequestStartEventPrefix(PersistentEventSystem __instance, long sender, int sourceEventId) {
    return false;
  }
}
