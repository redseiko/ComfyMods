namespace Queryable;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using ComfyLib;

using Database;

using SQLite;

using UnityEngine;

public static class DumpZDOsCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "dump-zdos",
        "(Queryable) dump-zdos",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    if (!PrefabUtils.TryGetPrefabCache(out Dictionary<int, GameObject> prefabCache)) {
      ComfyLogger.LogError("Could not find a valid prefab-cache!", args.Context);
      return false;
    }

    PrefabUtils.ProcessPrefabCache(prefabCache);

    args.Context.StartCoroutine(RunCoroutine(args.Context));

    return true;
  }

  public static IEnumerator RunCoroutine(Terminal context) {
    ConcurrentQueue<ZDO> zdos = new(ZDOMan.s_instance.m_objectsByID.Values);
    long startTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    string databaseName = $"{WorldUtils.GetWorldName(ZNet.m_world)}-zdos-{startTimeSeconds}.db";

    using (SQLiteConnection populateDatabase = DatabaseUtils.CreateDatabaseAndTables(databaseName)) {
      Task populateTask =
          Task.Run(() => DatabaseUtils.InsertPrefabHashes(populateDatabase, PrefabUtils.PrefabNames));

      while (!populateTask.IsCompleted) {
        yield return null;
      }
    }


    ComfyLogger.LogInfo($"Parsing {zdos.Count} ZDOs and dumping ZDOs to: {databaseName}", context);

    using SQLiteConnection database = DatabaseUtils.CreateDatabase(databaseName);

    int zdoCount = zdos.Count;
    int tenPercentMod = Mathf.CeilToInt(zdoCount / 10);
    int nextCount = zdoCount - tenPercentMod;

    Task task = Task.Run(() => RunThread(database, zdos));

    while (!task.IsCompleted) {
      yield return null;

      if (zdos.Count < nextCount) {
        int parsedCount = zdoCount - zdos.Count;
        nextCount = zdos.Count - tenPercentMod;
        ComfyLogger.LogInfo($"Parsed {parsedCount}/{zdoCount} ({parsedCount * 1f / zdoCount:P2}) ZDOs.");
      }
    }

    TimeSpan elapsedTime = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - startTimeSeconds);

    ComfyLogger.LogInfo($"Finished dumping ZDOs, elapsed time: {elapsedTime}", context);
  }

  public static void RunThread(SQLiteConnection database, ConcurrentQueue<ZDO> zdos) {
    database.BeginTransaction();

    while (zdos.TryDequeue(out ZDO zdo)) {
      InsertDataObject(database, zdo);
    }

    database.Commit();
  }

  static DataObject InsertDataObject(SQLiteConnection database, ZDO zdo) {
    DataObject dataObject = ZDOUtils.CreateDataObject(zdo);
    database.Insert(dataObject, typeof(DataObject));

    return dataObject;
  }
}
