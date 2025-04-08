namespace Scenic;

using System.Collections.Generic;

using UnityEngine;

public static class ZNetSceneManager {
  static readonly List<ZDO> _zdosToRemove = [];

  public static void RemoveObjects(ZNetScene netScene, List<ZDO> currentNearObjects, List<ZDO> currentDistantObjects) {
    byte b = (byte) (Time.frameCount & 255);

    for (int i = 0, count = currentNearObjects.Count; i < count; i++) {
      currentNearObjects[i].TempRemoveEarmark = b;
    }

    for (int i = 0, count = currentDistantObjects.Count; i < count; i++) {
      currentDistantObjects[i].TempRemoveEarmark = b;
    }

    ZDOMan zdoManager = ZDOMan.s_instance;
    Dictionary<ZDO, ZNetView> netSceneInstances = netScene.m_instances;
    List<ZNetView> netSceneTempRemoved = netScene.m_tempRemoved;

    netSceneTempRemoved.Clear();
    _zdosToRemove.Clear();

    foreach (KeyValuePair<ZDO, ZNetView> pair in netSceneInstances) {
      ZDO zdo = pair.Key;

      if (zdo == null) {
        continue;
      }

      ZNetView netView = pair.Value;

      if (!netView) {
        _zdosToRemove.Add(zdo);
        continue;
      }

      if (zdo.TempRemoveEarmark != b) {
        netSceneTempRemoved.Add(netView);
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
  }
}
