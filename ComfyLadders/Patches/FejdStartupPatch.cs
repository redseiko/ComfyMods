namespace ComfyLadders;

using System.Reflection;

using HarmonyLib;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.Awake))]
  [HarmonyPriority(Priority.Last)]
  static void AwakePostfix() {
    UnpatchBetterLadders();
  }

  static void UnpatchBetterLadders() {
    MethodInfo method = AccessTools.Method(typeof(AutoJumpLedge), nameof(AutoJumpLedge.OnTriggerStay));
    Patches patches = Harmony.GetPatchInfo(method);

    if (patches == default || !patches.Owners.Contains("BetterLadders")) {
      return;
    }

    ComfyLadders.LogInfo($"Unpatching BetterLadders...");

    PatchProcessor patchProcessor = new(default, method);
    patchProcessor.Unpatch(HarmonyPatchType.Prefix, "BetterLadders");
  }
}
