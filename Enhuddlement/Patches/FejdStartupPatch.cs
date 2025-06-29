namespace Enhuddlement;

using HarmonyLib;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.Awake))]
  [HarmonyPriority(Priority.Last)]
  static void AwakePostfix() {
    HarmonyUtils.UnpatchType(typeof(EnemyHud));
  }
}
