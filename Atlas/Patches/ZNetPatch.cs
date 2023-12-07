using System.Collections.Generic;

using HarmonyLib;

using UnityEngine;

namespace Atlas {
  [HarmonyPatch(typeof(ZNet))]
  static class ZNetPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ZNet.LoadWorld))]
    static void LoadWorldPostfix(ref ZNet __instance) {
      PluginLogger.LogInfo($"Finished loading world file. ZNet.m_netTime is: {__instance.m_netTime}");

      //CreateOrUpdateWorldMetadataZDOs(ZDOMan.s_instance);
    }

    public static readonly int MetadataPrefabHashCode = "_ZoneCtrl".GetStableHashCode();
    public static readonly int MetadataTagHashCode = "metadataTag".GetStableHashCode();
    public static readonly int SessionIdsHashCode = "sessionIds".GetStableHashCode();

    static void CreateOrUpdateWorldMetadataZDOs(ZDOMan zdoManager) {
      long sessionId = zdoManager.m_sessionID;
      List<ZDO> worldMetadataZDOs = new();

      foreach (ZDO zdo in zdoManager.m_objectsByID.Values) {
        if (ZDOExtraData.s_strings.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, string> stringValues)
            && stringValues.TryGetValue(MetadataTagHashCode, out string metadataTag)
            && metadataTag == "World") {
          worldMetadataZDOs.Add(zdo);
        }
      }

      if (worldMetadataZDOs.Count > 0) {
        foreach (ZDO zdo in worldMetadataZDOs) {
          string sessionIds = zdo.GetString(SessionIdsHashCode);
          PluginLogger.LogInfo($"Appending sessionId {sessionId} to existing sessionIds: {sessionIds}");

          zdo.Set(SessionIdsHashCode, $"{sessionIds},{sessionId}");
        }
      } else {
        ZDO zdo = zdoManager.CreateNewZDO(new Vector3(1000000f, 0f, 1000000f), MetadataPrefabHashCode);
        PluginLogger.LogInfo($"Created new World metadata ZDO ({zdo.m_uid}) and adding sessionId: {sessionId}");

        zdo.Persistent = true;
        zdo.SetPrefab(MetadataPrefabHashCode);
        zdo.Set(MetadataTagHashCode, "World");
        zdo.Set(SessionIdsHashCode, $"{sessionId}");
      }
    }
  }
}
