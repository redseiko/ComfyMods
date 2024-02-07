namespace AlaCarte;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(Game))]
static class GamePatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Game.Pause))]
  static void PausePostfix() {
    if (IsModEnabled.Value && DisableGamePauseOnMenu.Value) {
      Game.m_pause = false;
    }
  }
}
