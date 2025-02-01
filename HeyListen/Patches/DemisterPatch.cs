namespace HeyListen;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Demister))]
static class DemisterPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Demister.Awake))]
  static void AwakePostfix(ref Demister __instance) {
    if (!IsModEnabled.Value || !DemisterBallUseCustomSettings.Value) {
      return;
    }

    GameObject prefab = __instance.transform.root.gameObject;

    if (prefab.name.StartsWith("demister_ball", System.StringComparison.InvariantCulture)
        && !prefab.TryGetComponent(out DemisterBallControl _)) {
      prefab.AddComponent<DemisterBallControl>();
    }
  }
}
