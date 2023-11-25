using HarmonyLib;

using static ColorfulPieces.PluginConfig;

namespace ColorfulPieces {
  [HarmonyPatch(typeof(Game))]
  static class GamePatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Game.Start))]
    static void StartPostfix(Game __instance) {
      if (IsModEnabled.Value) {
        __instance.gameObject.AddComponent<PieceColorUpdater>();
      }
    }
  }
}
