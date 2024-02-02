namespace Atlas;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;

using HarmonyLib;

using UnityEngine;

[HarmonyPatch(typeof(ZDOMan))]
static class ZDOManPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZDOMan.Load))]
  static IEnumerable<CodeInstruction> LoadTranspiler1(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        // Save the position after ZDO loading.
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Ldstr, "Adding to Dictionary"))
        .SavePosition(out int destination)
        // Find the start position for loading ZDOs when version >= 31.
        .Start()
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Ldloc_3),
            new CodeMatch(OpCodes.Ldarg_1),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ZPackage), nameof(ZPackage.SetReader))))
        // Insert a branch to the position after ZDO loading when version < 31.
        .InsertBranchAndAdvance(OpCodes.Br, destination)
        .InstructionEnumeration();
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZDOMan.Load))]
  static IEnumerable<CodeInstruction> LoadTranspiler2(
      IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        // Add exception handling for duplicate ZDOs saved.
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ZDOMan), nameof(ZDOMan.m_objectsByID))),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ZDO), nameof(ZDO.m_uid))),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(
                OpCodes.Callvirt,
                AccessTools.Method(typeof(Dictionary<ZDOID, ZDO>), nameof(Dictionary<ZDOID, ZDO>.Add))))
        .ThrowIfInvalid("Could not patch AddObjectsById!")
        .SaveInstruction(offset: 2, out CodeInstruction ldLocS9)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ZDOMan), nameof(ZDOMan.m_objectsByID))),
            ldLocS9,
            Transpilers.EmitDelegate(AddObjectsByIdPreDelegate))
        .InstructionEnumeration();
  }

  static CodeMatcher SavePosition(this CodeMatcher matcher, out int position) {
    position = matcher.Pos;
    return matcher;
  }

  static CodeMatcher SaveInstruction(this CodeMatcher matcher, int offset, out CodeInstruction instruction) {
    instruction = matcher.InstructionAt(offset);
    return matcher;
  }

  static void AddObjectsByIdPreDelegate(Dictionary<ZDOID, ZDO> objectsById, ZDO zdo) {
    if (objectsById.Remove(zdo.m_uid)) {
      PluginLogger.LogWarning($"Duplicate ZDO {zdo.m_uid} detected, overwriting.");
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZDOMan.Load))]
  static void LoadPostfix(ref ZDOMan __instance) {
    PluginLogger.LogInfo($"Loading ZDO.timeCreated for {__instance.m_objectsByID.Count} ZDOs.");
    Stopwatch stopwatch = Stopwatch.StartNew();

    foreach (ZDO zdo in __instance.m_objectsByID.Values) {
      if (ZDOExtraData.s_longs.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, long> values)
          && values.TryGetValue(Atlas.TimeCreatedHashCode, out long timeCreated)) {
        ZDOExtraData.s_tempTimeCreated[zdo.m_uid] = timeCreated;
      } else if (ZDOExtraData.s_tempTimeCreated.TryGetValue(zdo.m_uid, out timeCreated)) {
        zdo.Set(Atlas.TimeCreatedHashCode, timeCreated);
      } else {
        ZDOExtraData.s_tempTimeCreated[zdo.m_uid] = 0L;
        zdo.Set(Atlas.TimeCreatedHashCode, 0L);
      }
    }

    stopwatch.Stop();
    PluginLogger.LogInfo($"Finished loading ZDO.timeCreated, time: {stopwatch.Elapsed}");
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZDOMan.RPC_ZDOData))]
  static IEnumerable<CodeInstruction> RPC_ZDODataTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldloc_3),
            new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ZDO), nameof(ZDO.Deserialize))))
        .ThrowIfInvalid("Could not patch ZDOMan.RPC_ZDOData()! (Deserialize)")
        .SaveInstruction(offset: 0, out CodeInstruction ldLocS12)
        .Advance(offset: 3)
        .InsertAndAdvance(
            ldLocS12,
            Transpilers.EmitDelegate(SetTimeCreatedDelegate))
        .InstructionEnumeration();
  }

  static void SetTimeCreatedDelegate(ZDO zdo) {
    if (ZDOExtraData.s_longs.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, long> values)
        && values.TryGetValue(Atlas.TimeCreatedHashCode, out long timeCreated)) {
      ZDOExtraData.s_tempTimeCreated[zdo.m_uid] = timeCreated;
    } else if (ZDOExtraData.s_tempTimeCreated.TryGetValue(zdo.m_uid, out timeCreated)) {
      zdo.Set(Atlas.TimeCreatedHashCode, timeCreated);
    } else {
      timeCreated = (long) (ZNet.m_instance.m_netTime * TimeSpan.TicksPerSecond);
      ZDOExtraData.s_tempTimeCreated[zdo.m_uid] = timeCreated;

      zdo.Set(Atlas.TimeCreatedHashCode, timeCreated);
      zdo.Set(Atlas.EpochTimeCreatedHashCode, DateTimeOffset.Now.ToUnixTimeSeconds());
    }

    if (!zdo.TryGetZDOID(Atlas.OriginalUidHashPair, out _)) {
      zdo.Set(Atlas.OriginalUidHashPair, zdo.m_uid);
    }
  }

  static bool TryGetZDOID(this ZDO zdo, KeyValuePair<int, int> hashPair, out ZDOID value) {
    if (ZDOExtraData.s_longs.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, long> values)
        && values.TryGetValue(hashPair.Key, out long userIdPart)
        && values.TryGetValue(hashPair.Value, out long idPart)) {
      value = new(userIdPart, (uint) idPart);
      return true;
    }

    value = ZDOID.None;
    return false;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(ZDOMan.CreateNewZDO), typeof(Vector3), typeof(int))]
  static void CreateNewZDOPostfix(ref ZDO __result) {
    ZDOID zdoid = __result.m_uid;
    long timeCreated = (long) (ZNet.m_instance.m_netTime * TimeSpan.TicksPerSecond);

    ZDOExtraData.s_tempTimeCreated[zdoid] = timeCreated;
    ZDOExtraData.Set(zdoid, Atlas.TimeCreatedHashCode, timeCreated);
    ZDOExtraData.Set(zdoid, Atlas.EpochTimeCreatedHashCode, DateTimeOffset.Now.ToUnixTimeSeconds());

    ZDOExtraData.Set(zdoid, Atlas.OriginalUidHashPair.Key, zdoid.UserID);
    ZDOExtraData.Set(zdoid, Atlas.OriginalUidHashPair.Value, zdoid.ID);
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZDOMan.ConnectSpawners))]
  static bool ConnectSpawnersPrefix(ref ZDOMan __instance) {
    PluginLogger.LogInfo($"Starting ConnectSpawners with caching.");
    Stopwatch stopwatch = Stopwatch.StartNew();

    Dictionary<ZDOID, ZDOConnectionHashData> spawned = new();
    Dictionary<int, ZDOID> targetsByHash = new();

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

    long sessionId = __instance.m_sessionID;
    int connectedCount = 0;
    int doneCount = 0;

    foreach (KeyValuePair<ZDOID, ZDOConnectionHashData> pair in spawned) {
      if (pair.Key.IsNone() || !__instance.m_objectsByID.TryGetValue(pair.Key, out ZDO zdo) || zdo == null) {
        continue;
      }

      zdo.SetOwner(sessionId);

      if (targetsByHash.TryGetValue(pair.Value.m_hash, out ZDOID targetZdoId) && pair.Key != targetZdoId) {
        connectedCount++;
        zdo.SetConnection(ZDOExtraData.ConnectionType.Spawned, targetZdoId);
      } else {
        doneCount++;
        zdo.SetConnection(ZDOExtraData.ConnectionType.Spawned, ZDOID.None);
      }
    }

    stopwatch.Stop();
    PluginLogger.LogInfo($"Connected {connectedCount} spawners ({doneCount} 'done'), time: {stopwatch.Elapsed}");

    return false;
  }
}
