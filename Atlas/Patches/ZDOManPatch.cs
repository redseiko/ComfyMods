namespace Atlas;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

[HarmonyPatch(typeof(ZDOMan))]
static class ZDOManPatch {
  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZDOMan.Load))]
  static IEnumerable<CodeInstruction> LoadTranspiler(
      IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchEndForward(
            new CodeMatch(OpCodes.Ldstr, "Adding to Dictionary"))
        .ThrowIfInvalid($"Could not patch ZDOMan.Load()! (adding-to-dictionary)")
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

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(ZDOMan.LoadChunks))]
  static IEnumerable<CodeInstruction> LoadChunksTranspiler(
      IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        // Add exception handling for duplicate ZDOs saved.
        .Start()
        .MatchEndForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ZDOMan), nameof(ZDOMan.m_objectsByID))),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ZDOMan), nameof(ZDOMan.m_objectsByID))),
            new CodeMatch(OpCodes.Callvirt))
        .ThrowIfInvalid($"Could not patch ZDOMan.LoadChunks()! (ensure-capacity)")
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ZDOMan), nameof(ZDOMan.m_objectsByID))),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ZDO), nameof(ZDO.m_uid))),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(
                OpCodes.Callvirt,
                AccessTools.Method(typeof(Dictionary<ZDOID, ZDO>), nameof(Dictionary<ZDOID, ZDO>.Add))))
        .ThrowIfInvalid("Could not patch ZDOMan.LoadChunks()! (add-objects-by-id)")
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

    ZDOManUtils.SetTimeCreatedFields(zdo);
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
        .ThrowIfInvalid("Could not patch ZDOMan.RPC_ZDOData()! (deserialize)")
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
    ZDOManUtils.SetTimeCreatedNewZDO(__result.m_uid);
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ZDOMan.ConnectSpawners))]
  static bool ConnectSpawnersPrefix(ZDOMan __instance) {
    ZDOManUtils.ConnectSpawners(__instance);
    return false;
  }
}
