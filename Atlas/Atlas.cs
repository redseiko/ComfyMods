using BepInEx;

using HarmonyLib;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Atlas {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class Atlas : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.atlas";
    public const string PluginName = "Atlas";
    public const string PluginVersion = "1.1.0";

    Harmony _harmony;

    public void Awake() {
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    [HarmonyPatch(typeof(ZNet))]
    class ZNetPatch {
      [HarmonyPrefix]
      [HarmonyPatch(nameof(ZNet.SaveWorld))]
      static bool SaveWorldPrefix(ref ZNet __instance, ref bool sync) {
        if (sync) {
          return true;
        }

        __instance.StartCoroutine(SaveWorldCoroutine(__instance));
        return false;
      }
    }

    [HarmonyPatch(typeof(ZDOMan))]
    class ZDOManPatch {
      [HarmonyPrefix]
      [HarmonyPatch(nameof(ZDOMan.Load))]
      static bool LoadPrefix(ref ZDOMan __instance, ref BinaryReader reader, ref int version) {
        reader.ReadInt64();
        uint nextUid = reader.ReadUInt32();

        LoadZdos(ref __instance, ref reader, ref version, ref nextUid);
        LoadDeadZdos(ref __instance, ref reader, ref nextUid);

        __instance.RemoveOldGeneratedZDOS();
        __instance.m_nextUid = nextUid;

        return false;
      }
    }

    static void LoadZdos(ref ZDOMan zdoMan, ref BinaryReader reader, ref int version, ref uint nextUid) {
      ZDOPool.Release(zdoMan.m_objectsByID);

      zdoMan.m_objectsByID.Clear();
      zdoMan.ResetSectorArray();

      int zdoCount = reader.ReadInt32();
      ZLog.Log($"Loading {zdoCount} ZDOs ... myid: {zdoMan.m_myid}, version: {version}");

      ZPackage package = new();
      Stopwatch stopwatch = Stopwatch.StartNew();

      for (int i = 0; i < zdoCount; i++) {
        try {
          ZDO zdo = ZDOPool.Create(zdoMan);
          zdo.m_uid = new(reader);

          int count = reader.ReadInt32();
          package.Load(reader.ReadBytes(count));

          zdo.Load(package, version);
          zdo.SetOwner(0L);

          if (zdoMan.m_objectsByID.ContainsKey(zdo.m_uid)) {
            ZLog.LogWarning($"Skipping duplicate ZDO-{i}: {zdo.m_uid}");
          } else {
            zdoMan.m_objectsByID[zdo.m_uid] = zdo;
            zdoMan.AddToSector(zdo, zdo.m_sector);
          }

          if (zdo.m_uid.userID == zdoMan.m_myid && zdo.m_uid.id >= nextUid) {
            nextUid = zdo.m_uid.id + 1U;
          }
        } catch (Exception exception) {
          ZLog.LogError($"Failed to load ZDO-{i}: {exception}");
        }
      }

      stopwatch.Stop();
      ZLog.Log($"Finished loading {zdoMan.m_objectsByID.Count} ZDOs in {stopwatch.ElapsedMilliseconds} ms.");
    }

    static void LoadDeadZdos(ref ZDOMan zdoMan, ref BinaryReader reader, ref uint nextUid) {
      zdoMan.m_deadZDOs.Clear();

      int deadZdoCount = reader.ReadInt32();
      ZLog.Log($"Loading {deadZdoCount} dead ZDOs ... ");

      for (int i = 0; i < deadZdoCount; i++) {
        ZDOID zdoid = new(reader);
        long ticks = reader.ReadInt64();

        zdoMan.m_deadZDOs[zdoid] = ticks;

        if (zdoid.m_userID == zdoMan.m_myid && zdoid.m_id >= nextUid) {
          nextUid = zdoid.m_id + 1U;
        }
      }

      ZLog.Log($"Loaded {zdoMan.m_deadZDOs.Count} dead ZDOs.");
      zdoMan.CapDeadZDOList();
    }

    static IEnumerator SaveWorldCoroutine(ZNet zNet) {
      yield return SaveWorldAsync(zNet).ToIEnumerator();
    }

    static async Task SaveWorldAsync(ZNet zNet) {
      await Task
          .Run(() => {
            PrepareZDOManSave(ref zNet.m_zdoMan);
            ZoneSystem.instance.PrepareSave();
            RandEventSystem.instance.PrepareSave();
            zNet.SaveWorldThread();
          })
          .ConfigureAwait(continueOnCapturedContext: false);
    }

    static void PrepareZDOManSave(ref ZDOMan zdoMan) {
      ZDOMan.SaveData saveData = new();

      saveData.m_myid = zdoMan.m_myid;
      saveData.m_nextUid = zdoMan.m_nextUid;
      saveData.m_zdos = CloneZdos(ref zdoMan);

      try {
        saveData.m_deadZDOs = new(zdoMan.m_deadZDOs);
      } catch (Exception exception) {
        ZLog.LogError($"Failed to clone dead ZDOs (skipping): {exception}");
        saveData.m_deadZDOs = new();
      }

      zdoMan.m_saveData = saveData;

    }
    static List<ZDO> CloneZdos(ref ZDOMan zdoMan) {
      List<ZDO> clonedZdos = new(capacity: zdoMan.m_objectsByID.Count);

      ZLog.Log($"Start cloning {clonedZdos.Capacity} ZDOs ...");
      Stopwatch stopwatch = Stopwatch.StartNew();

      for (int i = zdoMan.m_objectsBySector.Length - 1; i >= 0; i--) {
        List<ZDO> zdos = zdoMan.m_objectsBySector[i];

        if (zdos == null) {
          continue;
        }

        for (int j = zdos.Count - 1; j >= 0; j--) {
          ZDO zdo = zdos[j];

          if (zdo != null && zdo.m_zdoMan != null && zdo.m_persistent) {
            clonedZdos.Add(zdo.Clone());
          }
        }
      }

      List<Vector2i> keys = new(zdoMan.m_objectsByOutsideSector.Keys);

      foreach (Vector2i key in keys) {
        if (!zdoMan.m_objectsByOutsideSector.TryGetValue(key, out List<ZDO> zdos)) {
          continue;
        }

        for (int i = zdos.Count - 1; i >= 0; i--) {
          ZDO zdo = zdos[i];

          if (zdo != null && zdo.m_zdoMan != null && zdo.m_persistent) {
            clonedZdos.Add(zdo.Clone());
          }
        }
      }

      stopwatch.Stop();
      ZLog.Log($"Finished cloning {clonedZdos.Count} ZDOs in {stopwatch.ElapsedMilliseconds} ms.");

      return clonedZdos;
    }
  }

  public static class TaskExtensions {
    public static IEnumerator ToIEnumerator(this Task task) {
      while (!task.IsCompleted) {
        yield return null;
      }

      if (task.IsFaulted) {
        throw task.Exception;
      }
    }
  }
}