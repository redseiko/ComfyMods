namespace Atlas;

using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

[HarmonyPatch(typeof(ZDOMan))]
static class ZDOManPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZDOMan.FilterZDO))]
  static void FilterZDOPostfix(ZDO zdo) {
    ZDOManUtils.SetTimeCreatedFields(zdo);
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZDOMan.Load))]
  static IEnumerable<CodeInstruction> LoadTranspiler(
      IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        // Add exception handling for duplicate ZDOs saved.
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ZDOMan), nameof(ZDOMan.m_objectsByID))),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ZDO), nameof(ZDO.m_uid))),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(
                OpCodes.Callvirt,
                AccessTools.Method(typeof(Dictionary<ZDOID, ZDO>), nameof(Dictionary<ZDOID, ZDO>.Add))))
        .ThrowIfInvalid("Could not patch ZDOMan.Load()! (add-objects-by-id)")
        .Advance(offset: 5)
        .SetInstructionAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ZDOManPatch), nameof(AddObjectsByIdDelegate))))
        .InstructionEnumeration();
  }

  static void AddObjectsByIdDelegate(Dictionary<ZDOID, ZDO> objectsById, ZDOID uid, ZDO zdo) {
    if (objectsById.ContainsKey(uid)) {
      PluginLogger.LogWarning($"Duplicate ZDO ({uid}) detected, overwriting.");
    }

    objectsById[uid] = zdo;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZDOMan.RPC_ZDOData))]
  static IEnumerable<CodeInstruction> RPC_ZDODataTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldloc_3),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ZDO), nameof(ZDO.Deserialize))))
        .ThrowIfInvalid("Could not patch ZDOMan.RPC_ZDOData()! (Deserialize)")
        .SaveInstruction(offset: 0, out CodeInstruction ldLocS12)
        .Advance(offset: 3)
        .InsertAndAdvance(
            ldLocS12,
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(ZDOManUtils), nameof(ZDOManUtils.SetTimeCreatedDelegate))))
        .InstructionEnumeration();
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZDOMan.CreateNewZDO), typeof(Vector3), typeof(int))]
  static void CreateNewZDOPostfix(ZDO __result) {
    ZDOID zdoid = __result.m_uid;

    ZDOExtraData.Set(zdoid, Atlas.TimeCreatedHashCode, (long) (ZNet.m_instance.m_netTime * TimeSpan.TicksPerSecond));
    ZDOExtraData.Set(zdoid, Atlas.EpochTimeCreatedHashCode, DateTimeOffset.Now.ToUnixTimeSeconds());

    ZDOExtraData.Set(zdoid, Atlas.OriginalUidHashPair.Key, zdoid.UserID);
    ZDOExtraData.Set(zdoid, Atlas.OriginalUidHashPair.Value, zdoid.ID);
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZDOMan.ConnectSpawners))]
  static bool ConnectSpawnersPrefix(ZDOMan __instance) {
    PluginLogger.LogInfo($"Starting ConnectSpawners with caching.");

    Dictionary<ZDOID, ZDOConnectionHashData> spawned = [];
    Dictionary<int, ZDOID> targetsByHash = [];

    foreach (KeyValuePair<ZDOID, ZDOConnectionHashData> pair in ZDOExtraData.s_connectionsHashData) {
      if (pair.Value.m_type == ZDOExtraData.ConnectionType.Spawned) {
        spawned.Add(pair.Key, pair.Value);
      } else if (pair.Value.m_type ==
          (ZDOExtraData.ConnectionType.Portal
              | ZDOExtraData.ConnectionType.SyncTransform
              | ZDOExtraData.ConnectionType.Target)) {
        targetsByHash[pair.Value.m_hash] = pair.Key;
      }
    }

    PluginLogger.LogInfo($"Connecting {spawned.Count} spawners against {targetsByHash.Count} targets.");

    Dictionary<ZDOID, ZDO> objectsById = __instance.m_objectsByID;
    long sessionId = __instance.m_sessionID;
    int connectedCount = 0;
    int doneCount = 0;

    foreach (KeyValuePair<ZDOID, ZDOConnectionHashData> pair in spawned) {
      if (pair.Key.IsNone() || !objectsById.TryGetValue(pair.Key, out ZDO zdo) || zdo == null) {
        continue;
      }

      zdo.SetOwner(sessionId);

      if (targetsByHash.TryGetValue(pair.Value.m_hash, out ZDOID targetZDOID) && pair.Key != targetZDOID) {
        connectedCount++;
        zdo.SetConnection(ZDOExtraData.ConnectionType.Spawned, targetZDOID);
      } else {
        doneCount++;
        zdo.SetConnection(ZDOExtraData.ConnectionType.Spawned, ZDOID.None);
      }
    }

    PluginLogger.LogInfo($"Connected {connectedCount} spawners ({doneCount} 'done').");

    return false;
  }
}
