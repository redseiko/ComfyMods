namespace Scenic;

using System.Collections.Generic;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(ZNetScene))]
static class ZNetScenePatch {
  static readonly List<ZDO> _zdosToRemove = new();

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZNetScene.RemoveObjects))]
  static bool RemoveObjectsPrefix(ZNetScene __instance, List<ZDO> currentNearObjects, List<ZDO> currentDistantObjects) {
    if (!IsModEnabled.Value) {
      return true;
    }

    byte b = (byte) (Time.frameCount & 255);

    for (int i = 0, count = currentNearObjects.Count; i < count; i++) {
      currentNearObjects[i].TempRemoveEarmark = b;
    }

    for (int i = 0, count = currentDistantObjects.Count; i < count; i++) {
      currentDistantObjects[i].TempRemoveEarmark = b;
    }

    ZDOMan zdoManager = ZDOMan.s_instance;
    Dictionary<ZDO, ZNetView> netSceneInstances = __instance.m_instances;
    List<ZNetView> netSceneTempRemoved = __instance.m_tempRemoved;

    netSceneTempRemoved.Clear();
    _zdosToRemove.Clear();

    foreach (KeyValuePair<ZDO, ZNetView> pair in netSceneInstances) {
      if (pair.Key == null) {
        continue;
      }

      if (!pair.Value) {
        _zdosToRemove.Add(pair.Key);
        continue;
      }

      if (pair.Key.TempRemoveEarmark != b) {
        netSceneTempRemoved.Add(pair.Value);
      }
    }

    for (int i = 0, count = netSceneTempRemoved.Count; i < count; i++) {
      ZNetView netView = netSceneTempRemoved[i];

      _zdosToRemove.Add(netView.m_zdo);
      netView.m_zdo.Created = false;
      netView.m_zdo = null;

      UnityEngine.Object.Destroy(netView.gameObject);
    }

    for (int i = 0, count = _zdosToRemove.Count; i < count; i++) {
      ZDO zdo = _zdosToRemove[i];

      if (!zdo.Persistent && zdo.Owner) {
        zdoManager.m_destroySendList.Add(zdo.m_uid);
      }

      netSceneInstances.Remove(zdo);
    }

    netSceneTempRemoved.Clear();
    _zdosToRemove.Clear();

    return false;
  }
}
