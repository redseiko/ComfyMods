using BepInEx;
using BepInEx.Configuration;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using UnityEngine;

namespace TwoDayShipping {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class TwoDayShipping : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.twodayshipping";
    public const string PluginName = "TwoDayShipping";
    public const string PluginVersion = "1.1.0";

    static ConfigEntry<string> _prefabNamesToPrioritize;
    static readonly HashSet<int> _priorityPrefabHashCodes = new();

    Harmony _harmony;

    public void Awake() {
      _prefabNamesToPrioritize =
          Config.Bind(
              "Priority",
              "prefabNamesToPrioritize",
              "guard_stone,guard_stone_test",
              "Comma-separated list of prefab names to prioritize for sending.");

      _priorityPrefabHashCodes.UnionWith(
          _prefabNamesToPrioritize.Value
              .Split(',')
              .Select(p => p.Trim())
              .Where(p => !p.IsNullOrWhiteSpace())
              .Select(p => p.GetStableHashCode()));

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    static readonly Comparison<ZDO> _serverSendComparison = new(ServerSendCompare);
    static readonly Comparison<ZDO> _clientSendComparison = new(ClientSendCompare);

    static int ServerSendCompare(ZDO x, ZDO y) {
      bool xIsPrioritized =
          x.m_type == ZDO.ObjectType.Prioritized && x.m_owner != 0L && x.m_owner != ZDOMan.compareReceiver;

      bool yIsPrioritized =
          y.m_type == ZDO.ObjectType.Prioritized && y.m_owner != 0L && y.m_owner != ZDOMan.compareReceiver;

      if (xIsPrioritized && yIsPrioritized) {
        return x.m_tempSortValue.CompareTo(y.m_tempSortValue);
      }

      if (xIsPrioritized != yIsPrioritized) {
        return xIsPrioritized ? -1 : 1;
      }

      return x.m_type == y.m_type
          ? x.m_tempSortValue.CompareTo(y.m_tempSortValue)
          : y.m_type.CompareTo(x.m_type);
    }

    static int ClientSendCompare(ZDO x, ZDO y) {
      if (x.m_type == ZDO.ObjectType.Prioritized && y.m_type == ZDO.ObjectType.Prioritized) {
        return x.m_tempSortValue.CompareTo(y.m_tempSortValue);
      }

      if (x.m_type == ZDO.ObjectType.Prioritized) {
        return -1;
      }

      if (y.m_type == ZDO.ObjectType.Prioritized) {
        return 1;
      }

      return x.m_tempSortValue.CompareTo(y.m_tempSortValue);
    }

    [HarmonyPatch(typeof(ZDOMan))]
    class ZDOManPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(ZDOMan.AddToSector))]
      static void AddToSectorPostfix(ref ZDO zdo) {
        if (zdo != null && _priorityPrefabHashCodes.Contains(zdo.m_prefab)) {
          zdo.m_type = ZDO.ObjectType.Prioritized;
        }
      }

      [HarmonyTranspiler]
      [HarmonyPatch(nameof(ZDOMan.RPC_ZDOData))]
      static IEnumerable<CodeInstruction> RPC_ZDODataTranspiler(IEnumerable<CodeInstruction> instructions) {
        return new CodeMatcher(instructions)
            .MatchForward(
                useEnd: true, new CodeMatch(OpCodes.Callvirt, typeof(ZDO).GetMethod(nameof(ZDO.Deserialize))))
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_S, Convert.ToByte(13)),
                Transpilers.EmitDelegate<Action<ZDO>>(
                    zdo => {
                      if (_priorityPrefabHashCodes.Contains(zdo.m_prefab)) {
                        zdo.m_type = ZDO.ObjectType.Prioritized;
                      }
                    }))
            .InstructionEnumeration();
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(ZDOMan.ServerSortSendZDOS))]
      static bool ServerSortSendZDOSPrefix(
          ref ZDOMan __instance, ref List<ZDO> objects, ref Vector3 refPos, ZDOMan.ZDOPeer peer) {
        float time = Time.time;
        float refPosSqrMagnitude = refPos.sqrMagnitude;

        for (int i = 0, count = objects.Count; i < count; i++) {
          ZDO zdo = objects[i];

          if (peer.m_zdos.TryGetValue(zdo.m_uid, out ZDOMan.ZDOPeer.PeerZDOInfo zdoInfo)) {
            zdo.m_tempHaveRevision = true;
            zdo.m_tempSortValue = Mathf.Clamp(time - zdoInfo.m_syncTime, 0f, 100f) * 1.5f;
            zdo.m_tempSortValue =
                zdo.m_position.sqrMagnitude - refPosSqrMagnitude - (zdo.m_tempSortValue * zdo.m_tempSortValue);
          } else {
            zdo.m_tempHaveRevision = false;
            zdo.m_tempSortValue = zdo.m_position.sqrMagnitude - refPosSqrMagnitude - 22500f;
          }
        }

        ZDOMan.compareReceiver = peer.m_peer.m_uid;
        objects.Sort(_serverSendComparison);

        return false;
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(ZDOMan.ClientSortSendZDOS))]
      static bool ClientSortSendZDOSPrefix(ref ZDOMan __instance, ref List<ZDO> objects, ref ZDOMan.ZDOPeer peer) {
        float time = Time.time;

        for (int i = 0, count = objects.Count; i < count; i++) {
          ZDO zdo = objects[i];
          zdo.m_tempSortValue =
              peer.m_zdos.TryGetValue(zdo.m_uid, out ZDOMan.ZDOPeer.PeerZDOInfo zdoInfo)
                  ? Mathf.Clamp(time - zdoInfo.m_syncTime, 0f, 100f) * -1.5f
                  : -150f;
        }

        objects.Sort(_clientSendComparison);

        return false;
      }
    }
  }
}