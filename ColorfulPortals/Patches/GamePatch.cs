namespace ColorfulPortals;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Game))]
static class GamePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Game.Start))]
  static void StartPostfix(Game __instance) {
    if (IsModEnabled.Value) {
      __instance.gameObject.AddComponent<TeleportWorldColorUpdater>();
    }
  }
}
