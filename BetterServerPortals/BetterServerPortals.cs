using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using UnityEngine;

using static BetterServerPortals.PluginConfig;

namespace BetterServerPortals {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class BetterServerPortals : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.betterserverportals";
    public const string PluginName = "BetterServerPortals";
    public const string PluginVersion = "1.5.0";

    static ManualLogSource _logger;
    Harmony _harmony;

    void Awake() {
      _logger = Logger;
      BindConfig(Config);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    static readonly HashSet<ZDOID> _zdosToForceSend = new();
    static readonly Dictionary<string, ZDO> _zdoByTagCache = new();

    public static IEnumerator ConnectPortalsCoroutine(ZDOMan zdoMan) {
      LogInfo($"Starting ConnectPortals coroutine with cache...");

      WaitForSeconds waitInterval = new(ConnectPortalCoroutineWait.Value);
      Stopwatch stopwatch = Stopwatch.StartNew();

      while (true) {
        ConnectPortals(zdoMan);

        if (stopwatch.ElapsedMilliseconds >= 60000L) {
          LogInfo($"Processed {zdoMan.m_portalObjects.Count} portals.");
          stopwatch.Restart();
        }

        yield return waitInterval;
      }
    }

    public static void ConnectPortals(ZDOMan zdoMan) {
      long sessionId = ZDOMan.GetSessionID();

      _zdosToForceSend.Clear();
      _zdoByTagCache.Clear();

      foreach (ZDO zdo in zdoMan.m_portalObjects) {
        string portalTag = zdo.GetString(ZDOVars.s_tag, string.Empty);
        ZDOID targetZdoid = zdo.GetConnectionZDOID(ZDOExtraData.ConnectionType.Portal);

        if (targetZdoid.IsNone()) {
          if (portalTag != string.Empty) {
            _zdoByTagCache[portalTag] = zdo;
          }

          continue;
        }

        if (portalTag == string.Empty
            || !zdoMan.m_objectsByID.TryGetValue(targetZdoid, out ZDO targetZdo)
            || targetZdo.GetString(ZDOVars.s_tag, string.Empty) != portalTag) {
          zdo.SetOwner(sessionId);
          zdo.UpdateConnection(ZDOExtraData.ConnectionType.Portal, ZDOID.None);

          _zdosToForceSend.Add(zdo.m_uid);

          if (portalTag != string.Empty) {
            _zdoByTagCache[portalTag] = zdo;
          }
        }
      }

      foreach (ZDO zdo in zdoMan.m_portalObjects) {
        string portalTag = zdo.GetString(ZDOVars.s_tag, string.Empty);
        ZDOID portalTargetZdoid = zdo.GetConnectionZDOID(ZDOExtraData.ConnectionType.Portal);

        if (portalTag == string.Empty || !portalTargetZdoid.IsNone()) {
          continue;
        }

        if (!_zdoByTagCache.TryGetValue(portalTag, out ZDO matchingZdo) || matchingZdo == zdo) {
          continue;
        }

        ZDOID matchingPortalTargetZdoid = matchingZdo.GetConnectionZDOID(ZDOExtraData.ConnectionType.Portal);

        if (matchingPortalTargetZdoid.IsNone()) {
          zdo.SetOwner(sessionId);
          zdo.SetConnection(ZDOExtraData.ConnectionType.Portal, matchingZdo.m_uid);

          matchingZdo.SetOwner(sessionId);
          matchingZdo.SetConnection(ZDOExtraData.ConnectionType.Portal, zdo.m_uid);

          _zdosToForceSend.Add(zdo.m_uid);
          _zdosToForceSend.Add(matchingZdo.m_uid);

          LogInfo($"Connected portals: {zdo.m_uid} <-> {matchingZdo.m_uid}");
        }
      }

      if (_zdosToForceSend.Count > 0) {
        foreach (ZDOMan.ZDOPeer zdoPeer in zdoMan.m_peers) {
          zdoPeer.m_forceSend.UnionWith(_zdosToForceSend);
        }
      }
    }

    static void LogInfo(string message) {
      _logger.LogInfo($"[{DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo)}] {message}");
    }
  }
}