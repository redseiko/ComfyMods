namespace ColorfulPieces;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(StaticPhysics))]
static class StaticPhysicsPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(StaticPhysics.Awake))]
  static void Awake(StaticPhysics __instance) {
    if (IsModEnabled.Value && !__instance.gameObject.TryGetComponent(out PieceColor _)) {
      __instance.gameObject.AddComponent<PieceColor>();
    }
  }
}
