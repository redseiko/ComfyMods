namespace Scenic;

using System.Collections.Generic;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ZNetScene))]
static class ZNetScenePatch {
  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZNetScene.RemoveObjects))]
  static bool RemoveObjectsPrefix(ZNetScene __instance, List<ZDO> currentNearObjects, List<ZDO> currentDistantObjects) {
    if (IsModEnabled.Value) {
      ZNetSceneManager.RemoveObjects(__instance, currentNearObjects, currentDistantObjects);
      return false;
    }

    return true;
  }
}
