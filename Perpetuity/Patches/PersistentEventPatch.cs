namespace Perpetuity;

using HarmonyLib;

[HarmonyPatch(typeof(PersistentEventSystem.PersistentEvent))]
static class PersistentEventPatch {
  // ...
}
